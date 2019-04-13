
module OrdersSystem.UIFragments
open Suave.Form
open Suave.Html
open System.Net
open OrdersSystem.DataProvider
let local = Resource.Load ("resources-"+Settings.Localization+".xml")
let  makePairsOfAlist aList  = 
    let rec listOfElementsToListOfPairs param accumul   = 
        match param with
         | [] -> accumul
         | [A;B] -> accumul@[[Some A;Some B]]
         | [A] -> accumul@[[Some A; None]]
         | H::T -> 
            match T with
            | H1::T1 ->  
               listOfElementsToListOfPairs T1 ([Some H;Some H1]::accumul)
             | [] -> accumul
    listOfElementsToListOfPairs aList []

let  makeTriplesOfAlist2Back aList  = 
    let rec listOfElementsToListOfTriples param accumul   = 
        match param with
         | [] -> accumul
         | [A;B;C] -> accumul@[[Some A;Some B;Some C]]
         | [A;B] -> accumul@[[Some A;Some B; None]]
         | [A] -> accumul@[[Some A; None; None]]
         | H::T -> 
            match T with
            | H1::T1 ->  
             match T1 with
             | H2::T2 -> 
               listOfElementsToListOfTriples T2 ([Some H;Some H1;Some H2]::accumul)
             | [] -> accumul
    listOfElementsToListOfTriples aList []

let em s = tag "em" [] [Text s]
let cssLink href = link [ "href", href; " rel", "stylesheet"; " type", "text/css" ]
let h2 s = tag "h2" [] [Text s]
let meta = tag "meta" []
let ul nodes = tag "ul" [] nodes
let ulAttr attr nodes = tag "ul" attr nodes
let li = tag "li" []
let table x = tag "table" [] x
let th x = tag "th" [] x
let tr x = tag "tr" [] x
let td x = tag "td" [] x
let strong s = tag "strong" [] (text s)
let form x = tag "form" ["method", "POST"] x
let formInput = Suave.Form.input
let submitInput value = input ["type", "submit"; "value", value]

type Field<'a> = {
    Label : string
    Html : Form<'a> -> Suave.Html.Node
}

type Fieldset<'a> = {
    Legend : string
    Fields : Field<'a> list
}
type FormLayout<'a> = {
    Fieldsets : Fieldset<'a> list
    SubmitText : string
    Form : Form<'a>
}

let renderForm (layout : FormLayout<_>) =    

   form [
       for set in layout.Fieldsets -> 
           tag "fieldset" [] [
               yield tag "legend" [] [Text set.Legend]

               for field in set.Fields do
                   yield div ["class", "editor-label"] [
                       Text field.Label
                   ]
                   yield div ["class", "autocomplete"] [
                       field.Html layout.Form
                   ]
           ]

       yield submitInput layout.SubmitText
   ]


let addItemOfCategory (order:Db.Orderdetail) (categories:Db.CourseCategories list) backUrl = 
  let pairOfCategories = makePairsOfAlist categories
  table [for category in pairOfCategories -> 
    tr  [ for subItem in category -> 
        match subItem with 
        | Some theSubItem  -> 
            td [a ((sprintf Path.Orders.addOrderItemByCategory order.Orderid theSubItem.Categoryid (WebUtility.UrlEncode backUrl))) ["class","buttonX"] 
            [Text (" + " + theSubItem.Name + "  ")] ]
        | None ->  td []
 ]]

let deleteOrderItemLink (orderItem:Db.OrderItemDetails) (mapOfStates: Map<int,Db.State>) backUrl = 
    if (mapOfStates.[orderItem.Stateid].Isinitial) then
     a ((sprintf Path.Orders.deleteOrderItem orderItem.Orderitemid (WebUtility.UrlEncode (backUrl+"#order"+(orderItem.Orderid|> string))))) ["",""] [Text local.Remove]
    else em ""


let ordersBar (myOrders:Db.Orderdetail list) (otherOrders:Db.Orderdetail list) =
    [
        Text(local.MyOwn)
        tag "p" [] [
            for order in myOrders -> ( a (sprintf Path.Orders.viewOrder order.Orderid)  [] [Text (" Tav. " + order.Table)]) 
        ]
        Text(local.OfOthers)
        tag "p" [] [
            for order in otherOrders -> ( a (sprintf Path.Orders.viewOrder order.Orderid)  [] [Text (" Tav. " + order.Table)]) 
        ]
      ]


    

let modifyOrderItemLink (orderItem:Db.OrderItemDetails) (mapOfStates: Map<int,Db.State>) backUrl =
      if (mapOfStates.[orderItem.Stateid].Isinitial) then
       a ((sprintf Path.Orders.editOrderItemByCategory orderItem.Orderitemid orderItem.Categoryid (WebUtility.UrlEncode (backUrl+"#order"+(orderItem.Orderid|> string) ) ))) ["",""] [Text local.Modify]
      else em ""

let allOrdersLinkForUserView (userView:Db.UsersView) = 
    match userView.Rolename with
    | "admin" -> tag "p" []  [ a (Path.Orders.allOrders) ["class","buttonX"] [Text local.AllTheOrders]]
    | _ -> em  ""

let allergeneMarkVariationPrintForDetail (variationToPrint:Db.VariationDetail) =
    variationToPrint.Ingredientname + (if (variationToPrint.Allergen) then Globals.ALERGEN_MARK else "")

let goNextStateLink (orderItem:Db.OrderItemDetails) statesEnabledForUser (mapOfStates: Map<int,Db.State>) backUrl=
    let isUserEnabledForState stateId =
        statesEnabledForUser |> List.contains stateId
    if (isUserEnabledForState orderItem.Stateid)||(mapOfStates.[orderItem.Stateid].Isinitial)
        then
            a ((sprintf Path.Orders.moveOrderItemToTheNextStateAndGoMyOrders orderItem.Orderitemid (WebUtility.UrlEncode  (backUrl+"#order"+(orderItem.Orderid |> string))))) ["class","buttonX"] [Text ("conf. ")]
        else
            em ""

let removeOrderItemLink (orderItem:Db.OrderItemDetails) isEnabled backUrl =
    if (isEnabled) then
        a ((sprintf Path.Orders.removeOrderItemThenGoBackToUrl orderItem.Orderitemid (WebUtility.UrlEncode  (backUrl+"#order"+(orderItem.Orderid |> string))))) ["class","buttonX"] [Text (local.Remove)]
    else 
        em ""
        
    

let allergeneMarkVariationPrintForIngredientOfCourse (ingredientToPrint:Db.IngredientOfCourse) =
    ingredientToPrint.Ingredientname+(if (ingredientToPrint.Allergen) then "(*)" else "")

let allergeneMarkVariationPrintForIngredient (ingredientToPrint:Db.Ingredient) =
    ingredientToPrint.Name+(if (ingredientToPrint.Allergen) then "(*)" else "")

let unVoidMyLatest (userView:Db.UsersView) backUrl  =
    tag "p" [] [a (sprintf Path.Orders.unVoidLatestVoidedOrderOfUser userView.Userid (WebUtility.UrlEncode backUrl)) ["class","buttonX"] [Text local.RollBackLatestVoided]]

let myOrdersLink (userView:Db.UsersView) = 
    match userView.Rolename with
    | "admin" -> tag "p" []  [ a (Path.Orders.myOrders) ["class","buttonX"] [Text local.MyOwn]]
    | _ -> em  ""

let voidOrderLink orderId (userView:Db.UsersView) backUrl = 
      match userView.Canvoidorders with
        |  true ->   (a ((sprintf Path.Orders.voidOrderFromMyOrders orderId (WebUtility.UrlEncode backUrl) )) ["",""] [Text local.VoidOrder])
        | _ -> em ""



