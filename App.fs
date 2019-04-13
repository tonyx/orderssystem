module OrdersSystem.App

open System

open System.Linq
open Xceed.Words.NET
open System.Drawing
open FSharp.Configuration
open Suave
open Suave.Authentication
open Suave.Cookie
open Suave.Filters
open Suave.Form
open Suave.Model.Binding
open Suave.Operators
open Suave.RequestErrors
open Suave.State.CookieStateStore
open Suave.Successful
open Suave.Web
open Globals
open View
open FSharp.Data.Sql
open OrdersSystem
open Suave.Writers
open QRCoder
open System.Net
open OrdersSystem.DbWrappedEntities 
open System.IO
open System.Text

open System.Drawing;
open System.Drawing.Printing;
open System.Security.Cryptography.X509Certificates

type Settings = AppSettings<"App.config">

type stritem = {entry: string}


type indexNameRecord = {name: string; index: int}
type indexNameDataRecord = {index: int; name: string; data: string}
type nameDataRecord = {name: string; data: string}
type indexUnitMeasureMap = {index: int; unitmeasure: string}
type twoindexnamerecord = {index1: int; index2: int; enablers: stritem list;observers: stritem list}
type perrolecategories = {roleid: int; categories: string list}
type manyrolescategories = {rolecategories: perrolecategories}
type ordersLiquidModel = {orders:  OrderWrapped list; targetOrder: OrderWrapped}
type indexNameRecordList = {indexnameitems: indexNameRecord list }
type orderandorderitemslist = {order: OrderWrapped; orderitems: OrderItemDetailsWrapped list }
type orderandsuborderlist = {orderandorderitems: orderandorderitemslist list; targetorder: OrderWrapped}

let tenderCodes = [{index=1;name="CONTANTI"};{index=2;name="CREDITO"};{index=3;name="ASSEGNI"};{index=4;name="BUONI"};{index=5;name="CARTA DI CREDITO"}]

let log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

let reset =
   unsetPair SessionAuthCookie
   >=> unsetPair StateCookie
   >=> Redirection.FOUND Path.home



let session f = 
   statefulForSession
   >=> context (fun x -> 
       match x |> HttpContext.state with
       | None -> f NoSession
       | Some state ->
            
           match state.get "cartid", state.get "username", state.get "role", state.get "userid", state.get "roleid",state.get "temporary", state.get "canmanagecourses", state.get "canmanageallorders"  with 
           | Some cartId, None, None, None,None,None,None,None ->
               f (CartIdOnly cartId)
           | _, Some username, Some role, Some userid, Some roleid,Some temporary,Some canmanagecourses,Some canmanageallorders ->
               f (UserLoggedOn {Username = username; Role = role; UserId = userid;RoleId=roleid;Temporary=temporary;CanManageAllCourses=canmanagecourses;CanManageAllOrders=canmanageallorders})
           | _ -> 
               f NoSession)

let redirectWithReturnPath redirection =
   request (fun x ->
       let path = x.url.AbsolutePath
       Redirection.FOUND (redirection |> Path.withParam ("returnPath", path)))

let loggedOn f_success =
   authenticate
       Cookie.CookieLife.Session
       false
       (fun () -> Choice2Of2(redirectWithReturnPath Path.Account.logon))
       (fun _ -> Choice2Of2 reset)
       f_success

let canManageCourses f_success =
   loggedOn (session (function
       | UserLoggedOn { Role = "admin" } -> f_success
       | UserLoggedOn { CanManageAllCourses = true} -> f_success
       | UserLoggedOn _ -> FORBIDDEN "Only for admin or enabled users"
       | _ -> UNAUTHORIZED "Not logged in"
   ))

let canManageIngredients f_success =
   loggedOn (session (function
       | UserLoggedOn { Role = "admin" } -> f_success
       | UserLoggedOn { CanManageAllCourses = true} -> f_success
       | UserLoggedOn _ -> FORBIDDEN "Only for admin or enabled users"
       | _ -> UNAUTHORIZED "Not logged in"
   ))

let canManageIngredientsPassingUserLoggedOn f_success  =
   loggedOn (session (function
       | UserLoggedOn { Role = "admin"; Username=username; UserId = userid; RoleId=roleid; Temporary=false  } -> f_success { CanManageAllCourses = true; Role = "admin"; Username=username; UserId = userid; RoleId=roleid; Temporary=false; CanManageAllOrders = true  }
       | UserLoggedOn { CanManageAllCourses = true; Role=role; Username=username; UserId=userid; RoleId=roleid;Temporary=false; CanManageAllOrders = canManageAllOrders } -> f_success { CanManageAllCourses = true; Role=role; Username=username; UserId=userid; RoleId=roleid;Temporary=false;CanManageAllOrders=canManageAllOrders }
       | UserLoggedOn _ -> FORBIDDEN "Only for admin or enabled users"
       | _ -> UNAUTHORIZED "Not logged in"
   ))

let admin f_success =
   loggedOn (session (function
       | UserLoggedOn { Role = "admin" } -> f_success
       | UserLoggedOn _ -> FORBIDDEN "Only for admin"
       | _ -> UNAUTHORIZED "Not logged in"
   ))

let powerUser f_success =
   loggedOn (session (function
       | UserLoggedOn { Role = "powerUser" } -> f_success
       | UserLoggedOn { Role = "admin" } -> f_success
       | UserLoggedOn _ -> FORBIDDEN "Only for power users"
       | _ -> UNAUTHORIZED "Not logged in"
   ))

let adminWithUserName f_success =
   loggedOn (session (function
       | UserLoggedOn { Role = "admin";Username = X } -> f_success X
       | UserLoggedOn _ -> FORBIDDEN "Only for admin"
       | _ -> UNAUTHORIZED "Not logged in"
   ))

let powerUserPassingUserLoggedOn f_success =
   loggedOn (session (function
       | UserLoggedOn { Role = "powerUser";Username = username;UserId=userid;RoleId=roleid;Temporary=false;CanManageAllOrders=canManageAllOrders} -> f_success {Role="powerUser";Username=username;UserId=userid;RoleId=roleid;Temporary=false;CanManageAllCourses=true;CanManageAllOrders=canManageAllOrders}
       | UserLoggedOn { Role = "admin";Username = username;UserId=userid;RoleId=roleid;Temporary=false} -> f_success {Role="admin";Username=username;UserId=userid;RoleId=roleid;Temporary=false;CanManageAllCourses=true;CanManageAllOrders=true}
       | UserLoggedOn _ -> FORBIDDEN "Only for power users"
       | _ -> UNAUTHORIZED "Not logged in"
   ))

let temporaryUserPassingUserLoggedOn f_success =
    let ctx = Db.getContext()
    loggedOn (session (function
       | UserLoggedOn { Role = role;Username = username;UserId=userid;RoleId=roleid} -> 
        let dbUser = Db.Users.getUser(userid) ctx
        if (dbUser.Istemporary) then 
            f_success {Role=role;Username=username;UserId=userid;RoleId=roleid;Temporary=true;CanManageAllCourses=false;CanManageAllOrders=false} else FORBIDDEN "user is not enabled"
       | UserLoggedOn _ -> FORBIDDEN "Only for logged on users"
       | _ -> UNAUTHORIZED "Not logged in"
   ))

let anyUserExceptTemporary f_success =
   let ctx = Db.getContext()
   loggedOn (session (function
       | UserLoggedOn { Role = role;Username = username;UserId=userid;RoleId=roleid;Temporary=false} -> 
        let dbUser = Db.Users.getUser(userid) ctx
        if (dbUser.Enabled) then f_success {Role=role;Username=username;UserId=userid;RoleId=roleid;Temporary=false;CanManageAllCourses=dbUser.Canmanagecourses;CanManageAllOrders=dbUser.Canmanageallorders} else 
            FORBIDDEN "user is not enabled"
       | UserLoggedOn _ -> FORBIDDEN "Only for logged on users"
       | _ -> UNAUTHORIZED "Not logged in"
   ))

let anyUserPassingUserLoggedOn f_success =
   let ctx = Db.getContext()
   loggedOn (session (function
       | UserLoggedOn { Role = role;Username = username;UserId=userid;RoleId=roleid;Temporary=X} -> 
        let dbUser = Db.Users.getUser(userid) ctx
        if (dbUser.Enabled) then f_success {Role=role;Username=username;UserId=userid;RoleId=roleid;Temporary=X;CanManageAllCourses=dbUser.Canmanagecourses;CanManageAllOrders=dbUser.Canmanageallorders} else FORBIDDEN "user is not enabled"
       | UserLoggedOn _ -> FORBIDDEN "Only for logged on users"
       | _ -> UNAUTHORIZED "Not logged in"
   ))

let adminPassingUserLoggedOn f_success =
   loggedOn (session (function
       | UserLoggedOn { Role = "admin";Username = username;UserId=userid;RoleId=roleid} -> f_success {Role="admin";Username=username;UserId=userid;RoleId=roleid;Temporary=false;CanManageAllCourses=true;CanManageAllOrders=true}
       | UserLoggedOn _ -> FORBIDDEN "Only for admins"
       | _ -> UNAUTHORIZED "Not logged in"
   ))

let passHash (pass: string) =
    use sha = Security.Cryptography.SHA256.Create()
    Text.Encoding.UTF8.GetBytes(pass)
    |> sha.ComputeHash
    |> Array.map (fun b -> b.ToString("x2"))
    |> String.concat ""

let bindToForm form handler =
    bindReq (bindForm form) handler BAD_REQUEST

let sessionStore setF = context (fun x ->
    match HttpContext.state x with
    | Some state -> setF state
    | None -> never)

let returnPathOrHome = 
    request (fun x -> 
        let path = 
            match (x.queryParam "returnPath") with
            | Choice1Of2 path -> path
            | _ -> Path.home
        Redirection.FOUND path)

let makeOrderArchived id =
    let ctx = Db.getContext() 
    Db.archiveOrder id ctx
    Db.pushArchivedOrdersInLog id ctx
let authenticateUser (user : Db.UsersView) =
    authenticated Cookie.CookieLife.Session false 
    >=> session (function
        | CartIdOnly cartId ->
            let ctx = Db.getContext()
            sessionStore (fun store -> store.set "cartid" "")
        | _ -> succeed)
    >=> sessionStore (fun store ->
        store.set "username" user.Username
        >=> store.set "role" user.Rolename
        >=> store.set "userid" user.Userid
        >=> store.set "roleid" user.Role
        >=> store.set "temporary" user.Istemporary
        >=> store.set "canmanagecourses" user.Canmanagecourses
        >=> store.set "canmanageallorders" user.Canmanageallorders)

    >=> returnPathOrHome


let html container =
    // let ctx = Db.getContext()
    let result cartItems user = 
       OK (View.index container) >=> Writers.setMimeType "text/html; charset=utf-8" 

    session (function |_ -> result 0 None)

let sendImage bitmap =
    let result  =
        OK "ok" >=> Writers.setMimeType "image/bmp"
    session (function |_ -> bitmap)

let makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItemId ctx =
    log.Debug(sprintf "%s orerItemId: %d" "makeOrderItemAsRejectedIfContainsUnavalableIngredients" orderItemId)
    let orderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx
    let ingredientOfCourse = Db.getIngredientsOfACourse orderItemDetail.Courseid ctx
    let unavailableIngredients = ingredientOfCourse |> List.filter (fun (x:Db.IngredientOfCourse) -> x.Checkavailabilityflag && ((x.Availablequantity - (x.Quantity*(decimal)orderItemDetail.Quantity)) < (decimal)0))

    let _ = if (unavailableIngredients.Length>0) then
                let rejectionCause = unavailableIngredients |> List.fold (fun x (y:Db.IngredientOfCourse) -> x+ y.Ingredientname ) "mancano: "
                Db.SetOrderItemAsRejected orderItemId ctx
                Db.createRejectedOrderItem orderItemId orderItemDetail.Courseid rejectionCause ctx |> ignore
                ctx.SubmitUpdates()
            else ()
    ()

let makeOrderItemRejectedIfContainsInvisibleIngredients orderItemId ctx =
    let orderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx
    let ingredientOfCourse = Db.getIngredientsOfACourse orderItemDetail.Courseid ctx
    let invisibleIngredients = ingredientOfCourse |> List.filter (fun (x:Db.IngredientOfCourse ) -> (not x.Visibility || not x.Categoryvisibility)) 
    let _ = if (invisibleIngredients.Length>0) then
                let rejectionCause = invisibleIngredients |> List.fold (fun x (y:Db.IngredientOfCourse) -> x + y.Ingredientname ) "mancano: "
                Db.SetOrderItemAsRejected orderItemId ctx
                Db.createRejectedOrderItem orderItemId orderItemDetail.Courseid rejectionCause ctx |> ignore
                ctx.SubmitUpdates()
            else ()
    ()

let logonViaQrCode  = 
    log.Debug("logonViaQrCode")
    choose [
        GET >=>
            (let ctx = Db.getContext()
            request (fun r ->
                match r.queryParam "specialCode" with
                | Choice1Of2 name -> 
                     let user = Db.getTemporaryUserViewByName name ctx
                     match user with
                     | Some theUser ->
                        authenticateUser theUser
                     | None -> UNAUTHORIZED "not logged in"
                | Choice2Of2 msg -> BAD_REQUEST msg)
            )
        POST >=> 
            Redirection.FOUND Path.Orders.myOrders
    ] 

let logon =
    log.Debug("logon")
    choose [
       GET >=>
           (View.logon "" |> html)
       POST >=> bindToForm Form.logon (fun form ->
           let ctx = Db.getContext()
           let (Password password) = form.Password
           match Db.Users.validateUser(form.Username, passHash password) ctx with
           | Some user ->
                  authenticateUser user
           | _ ->
               View.logon "Username or password is invalid." |> html
       )
    ]
let deleteObjects = 
    View.objectDeletionPage |> html

let error = 
    View.notFound |> html

let deleteUsers = warbler (fun _ ->
    log.Debug("deleteUsers")
    let ctx = Db.getContext()
    let ordinaryUsers = Db.Users.getOrdinaryUsersView ctx |> List.filter  (fun (x:Db.UsersView) -> x.Rolename <> "admin")
    View.userDeletionPage ordinaryUsers Path.Admin.deleteUsers |> html
)

let deleteTemporaryUsers = warbler (fun _ ->
    log.Debug("deleteTemporaryUsers")
    let ctx = Db.getContext()
    let ordinaryUsers = Db.Users.getTemporaryUsersView ctx |> List.filter  (fun (x:Db.UsersView) -> x.Rolename <> "admin")
    View.userDeletionPage ordinaryUsers Path.Admin.deleteTemporaryUsers |> html
)

let deleteUserRoles = warbler (fun _ -> (
    log.Debug("deleteUserRoles")
    let ctx = Db.getContext()
    let allUserRoles = Db.getAllRoles ctx
    View.rolesDeletionPage allUserRoles |> html))


let alignAllPricesOfAnOrder orderId =
    log.Debug("alignAllPricesOfAnOrder")
    let ctx = Db.getContext()
    let order = Db.Orders.getOrder orderId ctx
    let orderItems = order.``public.orderitems by orderid``
    let plainTotal = orderItems |> Seq.map (fun (x:Db.OrderItem) -> x.Price) |> Seq.fold (fun acc x -> acc + x) ((decimal)0.0)
    let discountedPrice = 
        match (order.Adjustispercentage,order.Adjustisplain) with
        | (true,false) -> plainTotal + (order.Percentagevariataion)/((decimal)100.0)*plainTotal
        | (false,true) -> plainTotal + order.Plaintotalvariation
        | _ -> plainTotal
    order.Total <- plainTotal
    order.Adjustedtotal <- discountedPrice


let deleteIngredientsBySelection =
    log.Debug("deleteIngredientsBySelection")
    choose [
        GET >=> 
            warbler (fun _ ->
            let ctx = Db.getContext()
            let allIngredients = Db.getAllIngredients ctx   
            View.ingredientsDeletionPageBySelect allIngredients |> html)
        POST >=> bindToForm Form.ingredientDeletion  (fun form ->
            let ctx = Db.getContext()
            let ingredientName = form.IngredientName
            let ingredient = Db.tryGetIngredientByName ingredientName ctx
            match ingredient with
            | Some theCourse -> 
                Db.safeDeleteIngredient theCourse.Ingredientid ctx
                Redirection.FOUND Path.Admin.deleteIngredients
            | _ ->  Redirection.FOUND Path.Admin.deleteIngredients
        )
        ]

let deleteIngredients = warbler (fun _ ->
    log.Debug("deleteIngredients")
    let ctx = Db.getContext()
    let allIngredients = Db.getAllIngredients ctx
    View.ingredientsDeletionPage allIngredients |> html)

let deleteUserRole id =
    log.Debug("deleteUserRole")
    let ctx = Db.getContext()
    Db.safeDeleteRole id ctx
    Redirection.found Path.Admin.deleteObjects

let deleteIngredient id =
    log.Debug("deleteIngredient")
    let ctx = Db.getContext()
    Db.safeDeleteIngredient id ctx
    Redirection.found Path.Admin.deleteObjects 

let deletePrinter id =
    log.Debug("deletePrinter")
    let ctx = Db.getContext()
    Db.safeRemovePrinter id ctx
    Redirection.found Path.Admin.printers

let deleteIngredientPrice id =
    log.Debug("deleteIngredientPrice")
    let ctx = Db.getContext()
    let ingredientPrice = Db.getIngredientPrice id ctx
    let ingredientId = ingredientPrice.Ingredientid
    Db.safeDeleteIngredientPrice ingredientPrice ctx
    Redirection.found (sprintf Path.Admin.editIngredientPrices ingredientId)


let deleteCourseCategories =
    log.Debug("deleteCourseCategories")
    choose [
        GET >=> warbler (fun _ ->
            let ctx = Db.getContext()
            let allCourseCategories = Db.Courses.getAllourseCategories ctx
            View.courseCategoriesDeletionPage allCourseCategories |> html
        )
    ]

let deleteIngredientCategories  = warbler ( fun _ ->
    log.Debug("deleteIngredientCategories")
    let ctx = Db.getContext()
    let allIngredientCategories = Db.getAllIngredientCategories ctx
    View.ingredientCategoriesDeletionPage allIngredientCategories |> html
    )

let deleteCourses = 
    log.Debug("deleteCourses")
    choose [
        GET >=> 
            warbler (fun _ ->
            let ctx = Db.getContext()
            let allCourses = Db.Courses.getAllCourses ctx   
            View.coursesDeletionPage allCourses |> html)
        POST >=> bindToForm Form.courseDeletion  (fun form ->
            let ctx = Db.getContext()
            let courseName = form.CourseName
            let course = Db.Courses.tryGetCourseByName courseName ctx
            match course with
            | Some theCourse -> 
                Db.safeDeleteCourse theCourse ctx
                Redirection.FOUND Path.Admin.deleteCourses
            | _ ->  Redirection.FOUND Path.Admin.deleteCourses
        )
        ]

let emtpy (userLoggedOn:UserLoggedOnSession) = warbler (fun _ ->
    let ctx = Db.getContext()
    let user = Db.getUserById userLoggedOn.UserId ctx
    View.empty userLoggedOn user |> html
)

let controlPanel (userLoggedOn:UserLoggedOnSession) = warbler (fun _ ->
    log.Debug("controlPanel")
    let ctx = Db.getContext()
    let user = Db.getUserById userLoggedOn.UserId ctx
    View.controlPanel userLoggedOn user |> html
)

let userEnabledToSeeWholeDoneOrers userId = 
    log.Debug("userEnabledToSeeWholeDoneOrers")
    let ctx = Db.getContext()
    Db.isUserEnabledToSeeWholeOrders userId ctx
let categoriesManagement (userLoggedOn:UserLoggedOnSession) = warbler (fun _ ->
    log.Debug("categoriesManagement")
    let ctx = Db.getContext()
    let allCategories = Db.Courses.getAllCategories ctx
    View.coursesAndCategoriesManagement  allCategories |> html
)
let qrOrder (userLoggedOn:UserLoggedOnSession) = warbler (fun _ ->
    log.Debug("qrOrder")
    let ctx = Db.getContext()
    let orders = Db.Orders.getOngoingOrderDetailsByUser userLoggedOn.UserId ctx |> List.filter (fun (x:Db.Orderdetail) ->  x.Forqruserarchived<> true)
    let activeCategories = Db.Courses.getActiveCategories ctx

    let orderItemsOfOrders = orders |> 
        List.map (fun (x:Db.Orderdetail) -> (x.Orderid, Db.getOrderItemDetailOfOrderDetail x ctx)) 
        |> Map.ofList
    let mapOfStates = Db.getMapOfStates ctx

    let eventualRejectionsOfOrderItems = orders |>
         List.map (fun (x:Db.Orderdetail) ->  Db.getOrderItemDetailOfOrderDetail x ctx  ) |> List.fold (@) [] 
         |> List.map (fun (x:Db.OrderItemDetails) -> (x.Orderitemid,Db.getLatestRejectionOfOrderItem x.Orderitemid ctx)) |> Map.ofList

    View.qrOrder userLoggedOn orders activeCategories orderItemsOfOrders mapOfStates  eventualRejectionsOfOrderItems |> html)

let myOrders (userLoggedOn:UserLoggedOnSession) = warbler (fun _ ->
    log.Debug("myOrders")
    let ctx = Db.getContext()
    let orders = Db.Orders.getOngoingOrderDetailsByUser userLoggedOn.UserId ctx
    let userView = Db.getUserViewById (userLoggedOn.UserId) ctx
    let activeCategories = Db.Courses.getActiveCategories ctx

    let statesEnabledForUser = Db.listOfEnabledStatesForWaiter userView.Userid ctx

    let orderItemsOfOrders = orders |> List.map (fun (x:Db.Orderdetail) -> (x.Orderid, Db.getOrderItemDetailOfOrderDetail x ctx)) |> Map.ofList
    let mapOfLinkedStates = Db.getMapOfStates ctx 
    let backUrl = Path.Orders.myOrders
    let initialStateId = (Db.States.getInitState ctx).Stateid

    let outGroupsOfOrders = orders |> List.map (fun (x:Db.Orderdetail) -> 
        (x.Orderid,Db.getOutGroupsOfOrder x.Orderid ctx)) |> Map.ofList

    let eventualRejectionsOfOrderItems = orders |>
         List.map (fun (x:Db.Orderdetail) ->  Db.getOrderItemDetailOfOrderDetail x ctx  ) |> List.fold (@) [] 
         |> List.map (fun (x:Db.OrderItemDetails) -> (x.Orderitemid,Db.getLatestRejectionOfOrderItem x.Orderitemid ctx)) |> Map.ofList

    View.ordersList userView  orders activeCategories orderItemsOfOrders  mapOfLinkedStates statesEnabledForUser backUrl eventualRejectionsOfOrderItems initialStateId  outGroupsOfOrders |> html)


let myOrdersSingles (userLoggedOn:UserLoggedOnSession) = warbler (fun _ ->
    log.Debug("myOrdersSingles")
    let ctx = Db.getContext()
    let myOrders = Db.Orders.getOngoingOrderDetailsByUser userLoggedOn.UserId ctx
    let otherOrders = match userLoggedOn.CanManageAllOrders with 
                                        |  true -> Db.Orders.getOngoingOrderDetailsByAllUserExcept userLoggedOn.UserId ctx
                                        |  false -> []

    let userView = Db.getUserViewById (userLoggedOn.UserId) ctx
    let statesEnabledForUser = Db.listOfEnabledStatesForWaiter userView.Userid ctx
    let mapOfLinkedStates = Db.getMapOfStates ctx 
    let backUrl = Path.Orders.myOrders
    let initialStateId = (Db.States.getInitState ctx).Stateid
    let outGroupsOfOrders = myOrders |> List.map (fun (x:Db.Orderdetail) -> 
        (x.Orderid,Db.getOutGroupOfOrdeHavingSomeItemsInInitialState x.Orderid ctx)) |> Map.ofList
    View.ordersListbySingles userView  myOrders otherOrders  mapOfLinkedStates statesEnabledForUser backUrl  initialStateId  outGroupsOfOrders |> html)


let ordinaryUsers  = warbler (fun _ ->
    log.Debug("ordinaryUsers")
    let ctx = Db.getContext()
    let allUsers = Db.Users.getOrdinaryUsersView ctx
    View.userAdministrationPage  allUsers |> html)


let temporaryUsers  =  warbler (fun _ ->
    log.Debug("temporaryUsers")
    let ctx = Db.getContext()
    let temporaryUsers = Db.Users.getTemporaryUsersView ctx |> List.sortBy (fun (x:Db.UsersView) -> (x.Creationtime))
    View.temporaryUsersAdministrationPage  temporaryUsers |> html
)


let switchVisbilityOfIngredientCategory ingredientCatgoryId encodedBackUrl  =  
    log.Debug("switchVisbilityOfIngredientCategory")
    warbler (fun x ->
        let backUrl = WebUtility.UrlDecode encodedBackUrl
        let ctx = Db.getContext()
        let category = Db.tryGetIngredientCategory ingredientCatgoryId ctx
        let _ = match category with 
            | Some theCategory -> theCategory.Visibility <- (not theCategory.Visibility)
                                  ctx.SubmitUpdates()
            | _ -> ()
        Redirection.FOUND backUrl
    )


let switchVisbilityOfIngredient ingredientCategoryId ingredientId  =
    log.Debug("switchVisbilityOfIngredient")
    let ctx = Db.getContext()
    let ingredient = Db.tryGetIngredientById  ingredientId ctx
    let _ = match ingredient with 
        | Some theIngredient -> theIngredient.Visibility <- (not theIngredient.Visibility); 
                                ctx.SubmitUpdates()
        | _ -> ()
    let redirTo = sprintf  Path.Admin.editIngredientCategory ingredientCategoryId
    Redirection.FOUND redirTo

type ingredientnamemodel = {ingredientname: string}
let viewIngredientUsage ingredientId pageNumberBack =
    log.Debug("viewIngredientUsage")
    let ctx = Db.getContext()
    let ingredient = Db.getIngredientById ingredientId ctx
    choose [
        GET >=> warbler ( fun _ ->
            let o = {ingredientname = ingredient.Name} 
            DotLiquid.page("ingredientDecrementSearch.html")  o
        )

        POST  >=> bindToForm Form.date (
            fun form -> 
                let ingredientDecrements = Db.getIngredientDecrementsStartingFromDate ingredientId form.Date ctx
                (View.seeVariationOfIngredientStartingFromDate ingredient form.Date ingredientDecrements) |> html)
    ]


let fillIngredient id pageNumberBack (user:UserLoggedOnSession) =
    log.Debug("fillIngredient")
    let ctx = Db.getContext()
    let ingredient = Db.getIngredientById id ctx
    choose [
        GET >=> warbler (fun _ ->

            let visibleIngredientCategories = Db.getAllVisibleIngredientCategories ctx
            let theCategoryOfIngredient = Db.getIngredientCatgory ingredient.Ingredientcategoryid ctx
            let allSelectableCategories = 
                match theCategoryOfIngredient.Visibility with
                | true -> visibleIngredientCategories
                | false -> theCategoryOfIngredient::visibleIngredientCategories

            let oriCategory = ingredient.Ingredientcategoryid

            let backUrl = (sprintf  Path.Admin.editIngredientCategoryPaginated  oriCategory pageNumberBack)

            View.fillIngredient "" ingredient allSelectableCategories backUrl |> html)

        POST >=> bindToForm Form.ingredientLoad (fun form ->
                
                let quantity = form.Quantity
                let comment = form.Comment
                let increment = Db.createIngredientIncrement ingredient quantity ingredient.Unitmeasure comment user.UserId ctx
                let _ = match ingredient.Updateavailabilityflag with | true -> Db.addAmountOfingredientAvailability ingredient quantity ctx | _ -> ()
                Redirection.found (sprintf Path.Admin.editIngredientCategoryPaginated ingredient.Ingredientcategoryid pageNumberBack)
            )
    ]

let editIngredientPrices id =
    log.Debug("editIngredientPrices")
    let ctx = Db.getContext()
    let ingredient = Db.getIngredientById id ctx
    let ingredientPricesDetails = Db.getIngredientPricesDetails id ctx
    choose [
        GET >=> warbler (fun _ ->
            View.editIngredientPrices ingredient ingredientPricesDetails "" |> html
        )
        POST >=> bindToForm Form.ingredientPrice (fun form -> 
            let isDefaultSubtract = form.IsDefaultSubtract = "YES"
            let isDefaultAdd = form.IsDefaultAdd = "YES"
            match (Db.tryCreateIngredientPrice form.AddPrice id isDefaultAdd isDefaultSubtract form.Quantity form.SubtractPrice ctx) with
                | Some _ -> Redirection.found (sprintf Path.Admin.editIngredientPrices id)
                | None -> View.editIngredientPrices ingredient ingredientPricesDetails "esiste gia" |> html
        )
    ]

let editIngredient id  pageNumberBack =
    log.Debug(sprintf "%s %d" "editIngredient" id)
    choose [
        GET >=> warbler (fun _ ->
            let ctx = Db.getContext()
            let ingredient = Db.getIngredientById id ctx
            let visibleIngredientCategories = Db.getAllVisibleIngredientCategories ctx
            let theCategoryOfIngredient = Db.getIngredientCatgory ingredient.Ingredientcategoryid ctx
            let allSelectableCategories = 
                match theCategoryOfIngredient.Visibility with
                | true -> visibleIngredientCategories
                | false -> theCategoryOfIngredient::visibleIngredientCategories

            let ingredient = Db.getIngredientById id ctx
            let oriCategory = ingredient.Ingredientcategoryid
            let backUrl = (sprintf  Path.Admin.editIngredientCategoryPaginated  oriCategory pageNumberBack)
            View.editIngredient "" ingredient allSelectableCategories backUrl |> html)
        POST >=> bindToForm Form.ingredientEdit (fun form ->
            let ctx = Db.getContext()

            let existing = Db.findIngredientByName form.Name ctx
            match existing with
            | Some theExisting when (theExisting.Ingredientid <> id) ->
                let allIngredientOfCategory = Db.getallIngredientsOfACategory theExisting.Ingredientcategoryid ctx
                let category = Db.getCategoryOfIngredients theExisting.Ingredientcategoryid ctx
                let message = "il nome e' gia' usato"
                View.ingredientsOfACategory message category allIngredientOfCategory  |> html

            | _ ->
                let visibility = (form.Visibility = "VISIBLE")
                let description = match form.Comment with | Some X -> X | _ -> ""
                let ingredient = Db.getIngredientById id ctx
                ingredient.Name <- form.Name
                ingredient.Description <-description
                ingredient.Allergen <- (form.Allergene = "YES")
                ingredient.Visibility <- visibility
                let oriCategory = ingredient.Ingredientcategoryid
                ingredient.Ingredientcategoryid <- (int)form.Category
                ingredient.Updateavailabilityflag <- (form.UpdateAvailabilityFlag = "YES")
                ingredient.Checkavailabilityflag <- (form.CheckAvailabilityFlag = "YES")
                ingredient.Unitmeasure <- (form.UnitOfMeasure)
                ctx.SubmitUpdates()
                Redirection.found (sprintf Path.Admin.editIngredientCategoryPaginated oriCategory pageNumberBack)
            )
    ]

let editIngredientCategoryPaginated (idCategory:int) (pageNumber: int) =
    log.Debug(sprintf "%s %d %d" "editIngredientCategoryPaginated" idCategory pageNumber)
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            let ingredientsOfCurrentPage = Db.getAllIngredientsOfACategoryByPage idCategory pageNumber ctx
            let numberOfAllIngredientOfACategory = Db.getNumbeOfAllIngredientsOfACategory idCategory ctx
            let numberOfPages = (numberOfAllIngredientOfACategory - 1)/Globals.NUM_DB_ITEMS_IN_A_PAGE
            let ingredientCategory = Db.getCategoryOfIngredients idCategory ctx
            View.seeIngredientsOfACategoryPaginated ingredientCategory ingredientsOfCurrentPage numberOfPages pageNumber |> html)

        POST >=> bindToForm Form.searchIngredient (fun form ->
            let ingredientsOfCurrentpage = Db.getAllIngredientsOfACategoryByPageWithNameSearch idCategory 0 form.Name ctx
            let numberOfAllIngredientOfACategory = Db.getNumbeOfAllIngredientsOfACategory idCategory ctx
            let numberOfPages = (numberOfAllIngredientOfACategory - 1)/Globals.NUM_DB_ITEMS_IN_A_PAGE
            let ingredientCategory = Db.getCategoryOfIngredients idCategory ctx
            View.seeIngredientsOfACategoryPaginated ingredientCategory ingredientsOfCurrentpage numberOfPages 0 |> html
        )
    ]

let editIngredientCategory (idCategory: int) (user: UserLoggedOnSession) =
    log.Debug(sprintf "%s %d" "editIngredientCategory" idCategory)
    choose [
        GET >=> warbler (fun _ ->
            let ctx = Db.getContext()
            let allIngredientOfCategory = Db.getallIngredientsOfACategory idCategory ctx
            let category = Db.getCategoryOfIngredients idCategory ctx
            let allCategories = Db.getAllIngredientCategories ctx
            View.ingredientsOfACategory "" category allIngredientOfCategory  |> html)
        POST >=> bindToForm Form.ingredient (fun form ->
            let ctx = Db.getContext()
            match Db.findIngredientByName form.Name ctx with
            | Some existing ->
                let allIngredientOfCategory = Db.getallIngredientsOfACategory idCategory ctx
                let category = Db.getCategoryOfIngredients idCategory ctx
                let allCategories = Db.getAllIngredientCategories ctx
                let message = "ingrediente "+form.Name + " esiste già"
                View.ingredientsOfACategory message category allIngredientOfCategory   |> html
            | None ->
                let visibility = (form.Visibility = "VISIBLE")
                let allergene = (form.Allergene = "YES")
                let description = match form.Comment with | Some X -> X | _ -> ""
                let updateAvailabilityFlag = (form.UpdateAvailabilityFlag = "YES")
                let checkAvailabilityFlag = (form.CheckAvailabilityFlag = "YES")
                let unitOfMeasure = form.UnitOfMeasure

                let newIngredient = Db.newIngredient form.Name visibility description idCategory allergene form.AvailableQuantity  updateAvailabilityFlag checkAvailabilityFlag unitOfMeasure  ctx
                let _ =
                    match (int) (form.AvailableQuantity) with
                    | 0 -> ()
                    | _ -> let initialIncrement = Db.createIngredientIncrement newIngredient form.AvailableQuantity unitOfMeasure "inserimento iniziale" user.UserId ctx
                           ()
                Redirection.FOUND ((sprintf  Path.Admin.editIngredientCategoryPaginated idCategory 0)) 
            )
    ]

let visibleingredientCategories =
    log.Debug("visibleingredientCategories")
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun x ->
            let backUrl = match x.request.queryParam("returnPath") with | Choice1Of2 par -> par | _ -> Path.home
            let ctx = Db.getContext()
            let visibleIngredientCategories = Db.getVisibleIngredientCategories ctx
            View.visibleIngreientCategoriesAdministrationPage visibleIngredientCategories backUrl |> html)
        POST >=> bindToForm Form.ingredientCategory ( fun form -> 
            let visibility = (form.Visibility = "VISIBLE")  
            let description = match form.Comment with | Some X -> X | _ -> ""
            let _ = Db.newIngredientCatgory form.Name visibility description ctx
            Redirection.FOUND Path.Admin.allIngredientCategories
        )
    ]

type indexstringlist = {index:int; names: string list}

type myPrinterModel = {name: string; 
    coursecategories: indexNameRecord list;
    catenabledforcurrentprinter: indexNameRecord list;
    printerforstateenabled: indexNameRecord list;
    backurl: string;
    enabledstatesforcategories: indexstringlist list;
    selectedcategory: int;
    printername: string;
    printerid: int;
    isenabledforprintinginvoices: bool;
    isenabledforprintingreceipts: bool;
    }



let managePrinter printerId categoryId =
    log.Debug(sprintf "%s %d %d" "managePrinter" printerId categoryId)
    let ctx = Db.getContext()
    let printer = Db.getPrinter printerId ctx
    choose [
        GET >=>
        warbler (fun _ -> 
            let courseCategories = Db.Courses.getVisibleCourseCategories ctx
            let printerForCategories = Db.getPrinterForCategoriesDetails printerId ctx

            let stateMappingForPrinter = Db.getStatesPrinterMapping printerId ctx |>
                List.map (fun (x:Db.PrinterForCourseCategoryDetail) -> {index=x.Stateid;name = x.Statename})

            let idAndCategories = courseCategories |> 
                List.map (fun (x:Db.CourseCategories) -> {index=x.Categoryid;name=x.Name})

            let idAndEnabledCategories = printerForCategories |> 
                List.map (fun (x:Db.PrinterForCourseCategoryDetail) -> {index=x.Categoryid;name=x.Categoryname})

            let categoryStatesForCurrentPrinter = Db.getStatesPrinterMapping printerId ctx 
            let categoriesIds = categoryStatesForCurrentPrinter |> 
                 List.map (fun (x:Db.PrinterForCourseCategoryDetail) -> x.Categoryid) |> Set.ofList |> Set.toList

            let printerForReceiptAndInvoice = Db.tryGetPrinterForReceiptAndInvoice printerId ctx

            let (canPrintReceipt,canPrintInvoice) = match printerForReceiptAndInvoice with    
                | Some X -> (X.Printreceipt,X.Printinvoice)
                | None -> (false,false)

            let mapOfEnabledStatesForCategories = categoriesIds |> 
                List.map (fun i -> (i, categoryStatesForCurrentPrinter |> 
                    List.filter (fun (x:Db.PrinterForCourseCategoryDetail) -> x.Categoryid = i) |> 
                    List.map (fun (x:Db.PrinterForCourseCategoryDetail) -> x.Statename) ))
                    |> List.map (fun (i,j) -> {index=i;names=j})


            let selectedCategory = categoryId

            let o = {name = printer.Name;coursecategories = idAndCategories;
                catenabledforcurrentprinter=idAndEnabledCategories;
                printerforstateenabled = stateMappingForPrinter;
                backurl = Path.Admin.printers;
                enabledstatesforcategories = mapOfEnabledStatesForCategories;
                selectedcategory = selectedCategory;
                printername = printer.Name ;
                printerid = printer.Printerid;
                isenabledforprintinginvoices = canPrintInvoice;
                isenabledforprintingreceipts = canPrintReceipt
            }

            DotLiquid.page("printerStateCategoryMapping.html") o
        )

        POST >=> bindToForm Form.printerOutGroupForStates (fun form ->
            let categoryId = form.CategoryId
            let _ = 
                match form.TOBEWORKED with 
                | Some _ -> Db.createPrinterCategoryAssociation printerId ((int)categoryId) "TOBEWORKED"  ctx |> ignore
                | None -> Db.removePrinterCategoryAssociation printerId ((int)categoryId) "TOBEWORKED"  ctx |> ignore
            let _ = 
                match form.STARTEDWORKING with 
                | Some _ -> Db.createPrinterCategoryAssociation printerId ((int)categoryId) "STARTEDWORKING" ctx |> ignore
                | None  -> Db.removePrinterCategoryAssociation printerId ((int)categoryId) "STARTEDWORKING" ctx |> ignore
            let _ = 
                match form.READYFORDELIVERY with 
                | Some _ -> Db.createPrinterCategoryAssociation printerId ((int)categoryId) "READYFORDELIVERY" ctx |> ignore
                | None -> Db.removePrinterCategoryAssociation printerId ((int)categoryId) "READYFORDELIVERY" ctx |> ignore
            let _ = 
                match form.DELIVERED with 
                | Some _ -> Db.createPrinterCategoryAssociation printerId ((int)categoryId) "DELIVERED" ctx |> ignore
                | None -> Db.removePrinterCategoryAssociation printerId ((int)categoryId) "DELIVERED" ctx |> ignore
            let _ = 
                match form.DONE with 
                | Some _ -> Db.createPrinterCategoryAssociation printerId ((int)categoryId) "DONE" ctx |> ignore
                | None -> Db.removePrinterCategoryAssociation printerId ((int)categoryId) "DONE" ctx |> ignore

            let _ =
                match form.PRINTINVOICE with
                | Some _ -> Db.createOrUpdatePrinterInvoiceAssociation printerId ctx |> ignore
                | None -> Db.removePrinterInvoiceAssociationIfExists printerId ctx |> ignore

            let _ =
                match form.PRINTRECEIPT with
                | Some _ -> Db.createOrUpdatePrinterReceiptAssociation printerId ctx |> ignore
                | None -> Db.removePrinterReceiptAssociationIfExists printerId ctx |> ignore
            Redirection.FOUND (sprintf Path.Admin.managePrinter printerId ((int)categoryId)))
    ]

let recognizePrinters = 
    log.Debug("recognizePrinters")
    warbler (fun _ -> 
        let ctx = Db.getContext()
        let printerNames = [for i in System.Drawing.Printing.PrinterSettings.InstalledPrinters do yield i]
        printerNames |> List.iter (fun printerName -> Db.createPrinter printerName ctx)
        Redirection.FOUND Path.Admin.printers
    )


let resetPrinters =
    log.Debug("resetPrinters")
    warbler (fun _  ->
        let ctx = Db.getContext()
        Db.safeRemovePrinters ctx
        Redirection.FOUND Path.Admin.printers
    )

let adminPrinters =
    log.Debug("adminPrinters")
    let ctx = Db.getContext()
    warbler (fun _ ->
        let printers = Db.getPrinters ctx
        View.adminPrinters printers |> html
    )

let about = 
    log.Debug("about")
    warbler (fun _ ->
        View.about |> html
    )

let adminIngredientCategories = 
    log.Debug("adminIngredientCategories")
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            let ctx = Db.getContext()
            let allIngredientCategories = Db.getAllIngredientCategories ctx
            View.ingredientCatgoriesAdministrationPage allIngredientCategories  |> html)
        POST >=> bindToForm Form.ingredientCategory ( fun form -> 
            let visibility = (form.Visibility = "VISIBLE")  
            let description = match form.Comment with | Some X -> X | _ -> ""
            let _ = Db.newIngredientCatgory form.Name visibility description ctx
            Redirection.FOUND Path.Admin.allIngredientCategories
        )
    ]

let adminRoles  =  warbler (fun _ ->
    log.Debug("adminRoles")
    let ctx = Db.getContext()
    let allRoles = Db.getAllRoles ctx
    let allRolesWithObservers = Db.getObserverRoleStatusCategory ctx
    let allRolesWithEnablers = Db.getEnablerRoleStatusCategory ctx
    View.rolesAdministrationPage allRoles allRolesWithObservers allRolesWithEnablers |> html)

let adminCategories  = warbler (fun _ ->
    log.Debug("adminCategories")
    let ctx = Db.getContext()
    let coursesCategories = Db.Courses.getAllCategories ctx
    View.coursesAdministrationPage  coursesCategories |> html)

let allOrders (userLoggedOn:UserLoggedOnSession) = warbler (fun _ ->
    log.Debug("allOrders")
    let ctx = Db.getContext()
    let userView = Db.getUserViewById userLoggedOn.UserId ctx
    let activeCategories = Db.Courses.getActiveCategories ctx
    let orders = Db.Orders.getOngoingOrderdetails ctx
    let statesEnabledForUser = Db.listOfEnabledStatesForWaiter userView.Userid ctx
    let orderItemsOfOrders = orders |> List.map (fun (x:Db.Orderdetail) -> (x.Orderid, Db.getOrderItemDetailOfOrderDetail x ctx)) |> Map.ofList
    let mapOfStates = Db.getMapOfStates ctx
    let backUrl = Path.Orders.allOrders

    let initStateId = (Db.States.getInitState ctx).Stateid

    let outGroupsOfOrders = orders |> List.map (fun (x:Db.Orderdetail) -> 
        (x.Orderid,Db.getOutGroupsOfOrder x.Orderid ctx)) |> Map.ofList

    let eventualRejectionsOfOrderItems = orders |>
         List.map (fun (x:Db.Orderdetail) ->  Db.getOrderItemDetailOfOrderDetail x ctx  ) |> List.fold (@) [] 
         |> List.map (fun (x:Db.OrderItemDetails) -> (x.Orderitemid,Db.getLatestRejectionOfOrderItem x.Orderitemid ctx)) |> Map.ofList

    View.ordersList userView  orders activeCategories orderItemsOfOrders mapOfStates statesEnabledForUser  backUrl  eventualRejectionsOfOrderItems initStateId outGroupsOfOrders |> html)


let createEcrReceiptInstructionForSubOrder subOrderId orderId =
    log.Debug("createEcrReceiptInstructionForSubOrder")
    let ctx = Db.getContext()
    let subOrder = Db.Orders.getSubOrder subOrderId ctx
    let paymentItemDetailsOfSubOrder  = Db.Orders.getPaymentItemDetailsOfSubOrder subOrderId ctx
    let orderItemsOfSubOrder = Db.getOrderItemDetailsOfSubOrder subOrderId ctx
    let orderItemRows = orderItemsOfSubOrder |> List.map (fun (x:Db.OrderItemDetails) -> 
          (
            if (x.Quantity>1) then (sprintf Globals.ITALIAN_TEMPLATE_ROW_FOR_PAYMENT_ITEM x.Price x.Quantity x.Name ) 
            else (sprintf Globals.ITALIAN_TEMPLATE_ROW_FOR_UNITARY_PAYMENT_ITEM x.Price  x.Name ) 
          )
        )

    let paymentItemRows = paymentItemDetailsOfSubOrder |> List.map (fun x -> sprintf Globals.ITALIAN_TEMPLATE_ROW_FOR_PAYMENT_CLOSURE_ITEM ((int)x.Tendercodeidentifier) x.Amount)
    let outFile = new System.IO.StreamWriter(Settings.EcrFilePath,false,Encoding.ASCII)
    let _ = orderItemRows |> List.iter (fun x -> outFile.WriteLine(x))
    let _ = match subOrder.Subtotaladjustment with
        | X when X< 0M ->
                    outFile.WriteLine("SUBTOT")
                    outFile.WriteLine(sprintf "%s %.2f" "SCONT val=" ( -  subOrder.Subtotaladjustment))
        | X when X > 0M ->
                    outFile.WriteLine("SUBTOT")
                    outFile.WriteLine(sprintf "%s %.2f, term=34" "inp num=" (subOrder.Subtotaladjustment))
        | _ -> ()

    let _ = paymentItemRows |> List.iter (fun x -> outFile.WriteLine(x))
    let _ = outFile.Close()
    Db.setSubOrderAsPayed subOrderId ctx
    Redirection.found (sprintf Path.Orders.subdivideDoneOrder orderId)

let createEcrReceiptInstructionForOrder  orderId =
    log.Debug("createEcrReceiptInstructionForSubOrder")
    let ctx = Db.getContext()
    let paymentItemDetailsOfSubOrder  = Db.Orders.getPaymentItemDetailsOfOrder orderId ctx
    let order = Db.Orders.getOrder orderId ctx
    let orderItemsOfSubOrder = Db.getOrderItemDetailsOfOrder orderId ctx
    let orderItemRows = orderItemsOfSubOrder |> List.map (fun (x:Db.OrderItemDetails) -> 
          (
            if (x.Quantity>1) then (sprintf Globals.ITALIAN_TEMPLATE_ROW_FOR_PAYMENT_ITEM x.Price x.Quantity x.Name ) 
            else (sprintf Globals.ITALIAN_TEMPLATE_ROW_FOR_UNITARY_PAYMENT_ITEM x.Price  x.Name ) 
          )
        )

    let paymentItemRows = paymentItemDetailsOfSubOrder |> List.map (fun x -> sprintf Globals.ITALIAN_TEMPLATE_ROW_FOR_PAYMENT_CLOSURE_ITEM ((int)x.Tendercodeidentifier) x.Amount)
    let outFile = new System.IO.StreamWriter(Settings.EcrFilePath,false,Encoding.ASCII)
    let _ = orderItemRows |> List.iter (fun x -> outFile.WriteLine(x))
    let _ = match order.Total - order.Adjustedtotal with
                | X   when X > 0M  -> 
                    outFile.WriteLine("SUBTOT")
                    outFile.WriteLine(sprintf "%s %.2f" "SCONT val=" (order.Total - order.Adjustedtotal))
                | X   when X < 0M  -> 
                    outFile.WriteLine("SUBTOT")
                    outFile.WriteLine(sprintf "%s %.2f, term=34" "inp num=" (order.Adjustedtotal - order.Total))
                | _ -> ()

    let _ = paymentItemRows |> List.iter (fun x -> outFile.WriteLine(x))
    let _ = outFile.Close()

    makeOrderArchived orderId 
    Redirection.found  Path.Orders.seeDoneOrders 



let defaultActionableStatesForOrderOwner = warbler (fun _ ->
    log.Debug("defaultActionableStatesForOrderOwner")
    let ctx = Db.getContext()
    let states = Db.getOrderedListOfOrdinaryStates ctx
    let defaultActionableStatesForWaiter = Db.getActionableDefaultStatesForWaiter ctx
    View.defaultActionableStatesForWaiter states defaultActionableStatesForWaiter |> html)

let tempUserDefaultActionableStates = warbler (fun _ ->
    log.Debug("tempUserDefaultActionableStates")
    let ctx = Db.getContext()
    let states = Db.getOrderedListOfOrdinaryStates ctx
    let defaultActionableStatesForTempUser = Db.getDefaultActionableStatesForTempUser ctx
    View.defaultActionableStatesForTempUser states defaultActionableStatesForTempUser |> html)

let specificActionableStateForOrderOwner id = warbler (fun _ ->
    log.Debug("specificActionableStateForOrderOwner")
    let ctx = Db.getContext()
    let user = Db.Users.getUser id ctx

    let states = Db.getOrderedListOfOrdinaryStates ctx
    let actionableStatesForSpecificWaiter = Db.getActionableStatesForSpecificWaiter id ctx
    View.specificActionableStatesForWaiter user states actionableStatesForSpecificWaiter |> html)

let manageCourses = warbler (fun _ ->
    log.Debug("manageCourses")
    let ctx = Db.getContext()
    let allCourses = Db.Courses.getAllCourseDetails ctx
    View.seeVisibleCourses "categoria" allCourses  |> html)

let manageVisibleCoursesOfACategory categoryId =  
    log.Debug(sprintf "%s %d " "manageVisibleCoursesOfACategory" categoryId)
    let ctx = Db.getContext()
    let allCourses = Db.Courses.getVisibleCoursesDetailsByCatgory categoryId ctx
    let category = Db.Courses.tryGetCategoryById categoryId ctx
    View.seeVisibleCoursesOfACategory category allCourses |> html

let manageAllCoursesOfACategory categoryId =  
    log.Debug(sprintf "%s %d " "manageAllCoursesOfACategory" categoryId)
    let ctx = Db.getContext()
    let allCourses = Db.Courses.getAllCoursesDetailsByCatgory categoryId ctx
    let category = Db.Courses.tryGetCategoryById categoryId ctx
    View.seeAllCourses category allCourses |> html

let manageAllCoursesOfACategoryPaginated categoryId pageNumber =
    log.Debug(sprintf "%s %d %d" "manageAllCoursesOfACategoryPaginated" categoryId categoryId )
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            let coursesOfPage = Db.Courses.getAllCoursesDetailsByCategoryAndPage categoryId pageNumber ctx
            let numberOfAllCourses = Db.Courses.getNumberOfAllCoursesOfACategory categoryId ctx
            let numberOfPages = (numberOfAllCourses - 1)/Globals.NUM_DB_ITEMS_IN_A_PAGE

            let category = Db.Courses.tryGetCategoryById categoryId ctx
            View.seeAllCoursesPaginated category coursesOfPage numberOfPages pageNumber |> html)

        POST >=> bindToForm Form.searchCourse (fun form -> 
            let coursesOfPage = Db.Courses.getAllCoursesDetailsByCategoryWithTextNameSearch categoryId form.Name ctx
            let numberOfAllCourses = Db.Courses.getNumberOfAllCoursesOfACategory categoryId ctx
            let numberOfPages = (numberOfAllCourses - 1)/Globals.NUM_DB_ITEMS_IN_A_PAGE

            let category = Db.Courses.tryGetCategoryById categoryId ctx
            View.seeAllCoursesPaginated category coursesOfPage numberOfPages pageNumber |> html
        
        )
    ]

let manageVisibleCoursesOfACategoryPaginated categoryId pageNumber =
    log.Debug(sprintf "%s %d" "manageVisibleCoursesOfACategoryPaginated" categoryId)
    let ctx = Db.getContext()
    let coursesOfPage = Db.Courses.getVisibleCoursesDetailsByCategoryAndPage categoryId pageNumber ctx
    let numberOfAllCourses = Db.Courses.getNumberOfVisibleCoursesOfACatgory categoryId ctx
    let numberOfPages = (numberOfAllCourses - 1)/Globals.NUM_DB_ITEMS_IN_A_PAGE

    let category = Db.Courses.tryGetCategoryById categoryId ctx
    View.seeVisibleCoursesPaginated category coursesOfPage numberOfPages pageNumber |> html

let manageCategories = warbler (fun _ ->
    log.Debug("manageCategories")
    let ctx = Db.getContext()
    let allCategories = Db.Courses.getAllCategories ctx
    View.seeCategories allCategories |> html)

let manageVisibleCourses = warbler (fun _ ->
    log.Debug("manageVisibleCourses")
    let ctx = Db.getContext()
    let allCourses = Db.Courses.getAllCourseDetails ctx
    View.seeVisibleCourses "categoria" (List.filter (fun x -> x.Visibility)  allCourses) |> html) 

let editCourse id  =
   log.Debug(sprintf "%s %d ""editCourse" id)
   let ctx = Db.getContext()
   let courseCategories = Db.Courses.getVisibleCourseCategories ctx |> List.map (fun g -> decimal g.Categoryid,g.Name)

   let visibleIngredientCategories = Db.getVisibleIngredientCategories ctx

   let ingredientsOfTheCourse = Db.getIngredientsOfACourse id ctx

   match Db.Courses.tryFindCourse id ctx with
   | Some course ->
       choose [
           GET >=> warbler (fun _ ->
               html (View.editCourse course courseCategories visibleIngredientCategories 
                 ingredientsOfTheCourse ""))

           POST >=> bindToForm Form.course (fun form ->
               match (Db.Courses.tryFindCourseByName form.Name ctx) with
               | Some X when X.Courseid <> course.Courseid -> 
                "un piatto di nome "+ form.Name + "esiste già" |> 
                    View.editCourse course courseCategories visibleIngredientCategories 
                         ingredientsOfTheCourse |> html
               | _ ->
                   let desc = match form.Description with | Some d -> d | _ -> ""
                   Db.Courses.updateCourse 
                       course
                       form.Price 
                       form.Name 
                       desc   
                       (form.Visibile = "VISIBLE")
                       ((int)form.CategoryId) ctx
                   let retPath = sprintf Path.Courses.manageVisibleCoursesOfACategoryPaginated ((int)form.CategoryId) 0
                   Redirection.FOUND retPath
               )
       ]
   | None -> 
       never

let editCourseCategory id =
   log.Debug(sprintf "%s %d " "editCourseCategory" id)
   let ctx = Db.getContext()
   match Db.Courses.tryGetCourseCategory id ctx with
   | Some courseCategory ->
       choose [
           GET >=> warbler (fun _ ->
               html (View.editCourseCategory courseCategory ""))
           POST >=> bindToForm Form.courseCategoryEdit (fun form ->
               
               let categoryExists = Db.Courses.tryGetCourseCategoryByName form.Name ctx
               match categoryExists with
               | Some X when  (X.Categoryid <> id) -> 
                   "una categoria con il nome " + form.Name + " esiste già" |> (View.editCourseCategory courseCategory) |> html
               | _ ->
           
                   let visibility = match form.Visibility with "VISIBLE" -> true | _ -> false
                   let isAbstract = form.Abstract = "ABSTRACT"
                   Db.updateCourseCategory 
                        courseCategory
                        form.Name
                        visibility 
                        isAbstract
                        ctx
                   Redirection.FOUND Path.Courses.adminCategories)
       ]
   | None -> 
       never



let editUser id   =
    log.Debug(sprintf "%s %d " "editUser" id)
    let ctx = Db.getContext()
    match Db.tryFindUserById id ctx with
    | Some user ->
        choose [
            GET >=> warbler (fun _ ->
                html (View.editUser user))
            POST >=> bindToForm Form.userEdit   (fun form ->
                let enabled = match form.Enabled with | "YES" -> true | _ -> false
                let canVoidOrders = match form.CanVoidOrder with | "YES" -> true | _ -> false
                let canManageallOrders = match form.CanManageAllorders with | "YES" -> true | _ -> false
                let canChangeThePrices = form.CanChangeThePrices = "YES"
                let canManageAllCourses = form.CanManageAllCourses = "YES"

                Db.updateUserStatus id enabled canVoidOrders canManageallOrders canChangeThePrices canManageAllCourses ctx
                Redirection.FOUND Path.Admin.allUsers)
        ]
    | None -> never

let editTemporaryUser id   =
    log.Debug(sprintf "%s %d \n " "editTemporaryUser" id)
    let ctx = Db.getContext()
    match Db.tryFindUserById id ctx with
    | Some user ->
        choose [
            GET >=> warbler (fun (x:HttpContext) ->
                let hostAddress = Settings.HostAddress 
                html (View.editTemporaryUser user hostAddress ))
        ]
    | None -> never


let createCategory =
    choose [
        GET >=> (View.createCourseCategory ""  |> html)
        POST >=> bindToForm Form.courseCategoryEdit (fun form ->
            log.Debug("createCategory")
            let ctx = Db.getContext()
            match Db.Courses.tryFindCategoryByName form.Name ctx with
            | Some existing ->
                "Sorry this catgory name is already taken. Try another one."
                |> View.createCourseCategory
                |> html
            | None ->
               let categoryName  = form.Name
               let visibility = form.Visibility = "VISIBLE" 
               let isAbstract = form.Abstract = "ABSTRACT"
               Db.newCategory categoryName visibility isAbstract ctx
               Redirection.FOUND Path.Courses.adminCategories
            )
    ]

let setEnablerStatesForUser (statesId: int list) (userId:int) =
    log.Debug(sprintf "%s %d  " "setEnablerStatesForUser" userId)
    let ctx = Db.getContext()
    Db.setEnablerStatesForUser statesId userId ctx

let changePassword  (user:UserLoggedOnSession)= 
    let ctx = Db.getContext()
    
    choose [
        GET >=> (View.changePassword "" user |> html)
        POST >=>  bindToForm Form.changePassword  (fun form -> 
            let (Password password) = form.Password
            Db.updatePasswordOfUser user.UserId (passHash password) ctx
            Redirection.FOUND Path.home
        )
    ]

let randomAlphanumericString() =
    let chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"
    let random = new Random(System.DateTime.Now.Millisecond)
    seq {
        for i in {0..11} do
            yield chars.[random.Next(chars.Length)]
    } |> Seq.toArray |> (fun x -> new String(x))

let regenTempUser userId =
    log.Debug(sprintf "%s %d" "regenTempUser" userId)
    let ctx = Db.getContext()
    choose [
        GET >=> (View.recycleQrUser |> html) 
        POST >=> bindToForm Form.qrUser (fun form ->
            let dbUser = Db.Users.getUser userId ctx
            let newUserName = randomAlphanumericString()
            dbUser.Username <- newUserName
            dbUser.Creationtime <- System.DateTime.Now
            dbUser.Table <- form.TableName

            let existingOrders = Db.getAllOrdersOfUser userId ctx
            existingOrders |> Seq.iter (fun (x:Db.Order) -> x.Forqruserarchived <- true) 
            let order = Db.createOrderByUser form.TableName "" userId ctx
            ctx.SubmitUpdates()

            Redirection.FOUND Path.Admin.temporaryUsers
        )
    ]


let addQrUser = 
    log.Debug("addQrUser")
    let ctx = Db.getContext()
    choose [
        GET >=> (View.addQrUser |> html)
        POST >=> bindToForm Form.qrUser (fun form ->
            let temporaryRole = Db.getRoleByName "temporary" ctx
            let userName  =  randomAlphanumericString()
            let canManageAllOrders = false
            let password = ""
            let canChangeThePrices = false
            let canManageCourses = false

            let defaultTempUserEnablingStates = Db.getDefaultActionableStatesForTempUser ctx

            let user = Db.newUser canChangeThePrices canManageAllOrders userName  password temporaryRole.Roleid canManageCourses ctx
            user.Creationtime <- System.DateTime.Now
            user.Istemporary<- true
            user.Table <- form.TableName
            ctx.SubmitUpdates()

            let order = Db.createOrderByUser form.TableName "" user.Userid ctx

            let actionablesStatesIds = List.map (fun (x:Db.TempUserDefaultActionableStates) -> x.Stateid) defaultTempUserEnablingStates

            Db.setEnablerStatesForUser actionablesStatesIds user.Userid ctx

            Redirection.FOUND Path.Admin.temporaryUsers)
    ]




let addUser = 
    log.Debug("addUser")
    let ctx = Db.getContext()

    choose [

        GET >=> warbler (fun _ ->  (View.register ((Db.getRoles ctx)|> List.filter (fun (x:Db.Role) -> x.Rolename <> "admin")  )"" |> html))

        POST >=> bindToForm Form.register (fun form ->
            let roles = Db.getRoles ctx
            let ctx = Db.getContext()
            match Db.tryFindUsersViewByName form.Username ctx with
            | Some existing ->
                "Sorry this username is already taken. Try another one."
                |> View.register roles
                |> html
            | None ->
               let (Password password) = form.Password
               let role = form.Role

               let dbRole = Db.getRole ((int)role) ctx
               if (dbRole.Rolename = "admin") then failwith "can't add another admin"

               let defaultEnablingStates = Db.getActionableDefaultStatesForWaiter ctx
               let canmanageOrders = form.CanManageAllorders = "YES"
               let canChangeThePrices = form.CanChangeThePrices = "YES"
               let canManageAllCourses = form.CanManageAllCourses = "YES"

               let user = Db.newUser canChangeThePrices canmanageOrders form.Username  (passHash password) ((int) role) canManageAllCourses ctx
               let actionablesStatesIds = List.map (fun (x:Db.DefaultActionableState) -> x.Stateid) defaultEnablingStates
               Db.setEnablerStatesForUser actionablesStatesIds user.Userid ctx
               
               Redirection.FOUND Path.Admin.allUsers
            )
    ]

let getViableGroupOutIdentifiers orderId =
    log.Debug("getViableGroupOutIdentifiers")
    let ctx = Db.getContext()
    let outGroupsOfOrder = Db.getOutGroupsOfOrder orderId ctx
    let alreadyPrintedGroupsOfOrder = outGroupsOfOrder |> List.filter (fun (x:Db.OrderOutGroup) -> x.Printcount > 0) |> 
        List.map  (fun (x:Db.OrderOutGroup) -> x.Groupidentifier)
    let availableNumbers = [1..13] |> List.filter (fun i -> not (List.contains i alreadyPrintedGroupsOfOrder))

    availableNumbers |> List.map (fun x -> ((decimal) x,(string)x))


let addOrderItemByCategoryForOrdinaryUsers orderId categoryId backUrl (user:UserLoggedOnSession) =
    log.Debug("addOrderItemByCategoryForOrdinaryUsers")

    let ctx = Db.getContext()
    let anOrder = Db.Orders.tryGetOrder orderId ctx

    let viableGroupOutIdsForOrderItem = getViableGroupOutIdentifiers orderId

    match anOrder with
    | Some theOrder  ->
         choose [
          GET >=> warbler (fun x ->
            let visibleCourses  =
                Db.Courses.getVisibleCoursesByCategory categoryId ctx

            let coursesIdWithPrices =
                visibleCourses |> List.map (fun g -> decimal g.Courseid, g.Price |> string)

            let coursesNames = 
                Db.Courses.getVisibleCoursesByCategory categoryId ctx
                |> List.map (fun g -> decimal g.Courseid, g.Name)

            html (View.addOrderItem coursesNames coursesIdWithPrices backUrl viableGroupOutIdsForOrderItem))

          POST >=> bindToForm Form.orderItem (fun form ->
            let orderItem = Db.createOrderItemByCourseName form.CourseByName orderId ((int)(form.Quantity))  form.Comment form.Price form.GroupOut  ctx
            makeOrderItemRejectedIfContainsInvisibleIngredients orderItem.Orderitemid ctx
            makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItem.Orderitemid ctx

            Redirection.FOUND (backUrl+"#order"+(orderId|> string))
        )
       ]
      | _ -> Redirection.FOUND (Path.Orders.myOrders+"#order"+(orderId|> string))
     
let addOrderItemByCategoryForStrippedUsers orderId categoryId backUrl (user:UserLoggedOnSession)=
    log.Debug("addOrderItemByCategoryForStrippedUsers")

    let ctx = Db.getContext()
    let anOrder = Db.Orders.tryGetOrder orderId ctx

    let viableGroupOutIdsForOrderItem = getViableGroupOutIdentifiers orderId

    match anOrder with
    | Some theOrder  ->
         choose [
          GET >=> warbler (fun x ->
            let visibleCourses  =
                Db.Courses.getVisibleCoursesByCategory categoryId ctx

            let coursesIdWithPrices =
                visibleCourses |> List.map (fun g -> decimal g.Courseid, g.Price |> string)

            let coursesNames = 
                Db.Courses.getVisibleCoursesByCategory categoryId ctx
                |> List.map (fun g -> decimal g.Courseid, g.Name)

            html (View.addOrderItemForStrippedUsers  coursesNames coursesIdWithPrices backUrl viableGroupOutIdsForOrderItem)
          )

          POST >=> bindToForm Form.strippedOrderItem (fun form ->
            let course = Db.getCourseDetail ((int)form.CourseId) ctx
            
            let orderItem = Db.createOrderItemByCourseName form.CourseByName orderId ((int)(form.Quantity))  form.Comment course.Price form.GroupOut  ctx
            makeOrderItemRejectedIfContainsInvisibleIngredients orderItem.Orderitemid ctx
            makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItem.Orderitemid ctx


            Redirection.FOUND (backUrl+"#order"+(orderId|> string))
        )
       ]
      | _ -> Redirection.FOUND (Path.Orders.myOrders+"#order"+(orderId|> string))
     

let addOrderItemByCategoryPassingUserLoggedOn orderId categoryId urlEncodedBackUrl (user:UserLoggedOnSession) = 
    log.Debug("addOrderItemByCategoryPassingUserLoggedOn")
    warbler (fun (x:HttpContext) ->
        let ctx = Db.getContext()
        let dbUser = Db.getUserById user.UserId ctx
        match dbUser.Canchangetheprice with
        | true -> addOrderItemByCategoryForOrdinaryUsers orderId categoryId urlEncodedBackUrl (user:UserLoggedOnSession)
        | false -> addOrderItemByCategoryForStrippedUsers orderId categoryId urlEncodedBackUrl  (user:UserLoggedOnSession)
    )

let deleteOrderItem orderItemId =
    let ctx = Db.getContext()
    Db.deleteOrderItem orderItemId ctx
    Redirection.FOUND Path.Orders.myOrders

let deleteOrderItemByUser orderItemId encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug("deleteOrderItemByUser")
    warbler (fun x ->
        let backUrl = WebUtility.UrlDecode encodedBackUrl
        let ctx = Db.getContext()
        let theOrderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx
        let state = Db.States.getState(theOrderItemDetail.Stateid) ctx
        if ((theOrderItemDetail.Userid = user.UserId || user.Role = "admin") && (state.Isinitial))
                     then Db.deleteOrderItem orderItemId ctx 
        Redirection.FOUND backUrl
    )

let setStateAsActionableForTempUserByDefault stateId =
    log.Debug("setStateAsActionableForTempUserByDefault")
    let ctx = Db.getContext()
    let _ = Db.createTempUserActionableState stateId ctx
    Redirection.FOUND Path.Admin.tempUserDefaultActionableStates

let setStateAsActiobableByDefault stateid =
    log.Debug(sprintf "%s %d " "setStateAsActiobableByDefault" stateid)
    let ctx = Db.getContext()
    let _ = Db.createWaiterActionableState stateid ctx
    Redirection.FOUND Path.Admin.defaultActionableStatesForOrderOwner

let unSetStateAsActiobableByDefault stateid =
    let ctx = Db.getContext()
    let _ = Db.deleteWaiterActionableState stateid ctx
    Redirection.FOUND Path.Admin.defaultActionableStatesForOrderOwner

let unSetStateAsActionableForTempUserByDefault stateid =
    let ctx = Db.getContext()
    let _ = Db.deleteDefaultTempUserActionableState stateid ctx
    Redirection.FOUND Path.Admin.tempUserDefaultActionableStates

let setStateAsActionableForWaiter userId stateId =
    let ctx = Db.getContext()
    let _ = Db.createSpecificWaiterAcionableState userId stateId ctx
    let redirection = sprintf Path.Admin.actionableStatesForSpecificOrderOwner userId
    Redirection.FOUND redirection

let unSetStateAsAcionableForWaiter userId stateId =
    let ctx = Db.getContext()
    let _ = Db.deleteSpecificWaiterAcionableState userId stateId ctx
    let redirection = sprintf Path.Admin.actionableStatesForSpecificOrderOwner userId
    Redirection.FOUND redirection

let deleteEnablerMapping id =
    let ctx = Db.getContext()
    Db.deleteEnabler id ctx
    Redirection.FOUND Path.Admin.roles

let deleteObserverMapping id =
    let ctx = Db.getContext()
    Db.deleteObserver id ctx
    Redirection.FOUND Path.Admin.roles

let canMoveOrderItemToNextStep (orderItemDetail:Db.OrderItemDetails) (user:UserLoggedOnSession) =
    let ctx = Db.getContext()
    let stateEnableForUserAsWaiter = Db.listOfEnabledStatesForWaiter user.UserId ctx
    ((   Db.userEnabledByRoletoChangeStateOfThisOrderItem orderItemDetail user.UserId ctx ) ||
        (((List.contains orderItemDetail.Stateid stateEnableForUserAsWaiter) &&
           (not orderItemDetail.Hasbeenrejected) )))
let moveOrderItemNextState (orderItemDetail:Db.OrderItemDetails) (user:UserLoggedOnSession)  =
    let ctx = Db.getContext()
    let stateEnableForUserAsWaiter = Db.listOfEnabledStatesForWaiter user.UserId ctx
    match (Db.userEnabledByRoletoChangeStateOfThisOrderItem orderItemDetail user.RoleId ctx || 
            (orderItemDetail.Userid = user.UserId && (List.contains orderItemDetail.Stateid stateEnableForUserAsWaiter 
            && (not orderItemDetail.Hasbeenrejected)
            ))) with
    | true -> 
        Db.tryMoveOrderItemToNextState orderItemDetail.Orderitemid user.UserId ctx; 
    | false -> () 

let moveAllOrderItemsInBlockFromInitialState orderId (user:UserLoggedOnSession) =
    let ctx = Db.getContext()
    let orderItemsDetails = Db.getOrderItemDetailOfOrderById orderId ctx
    let orderItemsToMove = orderItemsDetails |> 
        List.filter (fun (x:Db.OrderItemDetails) -> (Db.isInitialState x.Stateid ctx))
    let _ = orderItemsToMove |> Seq.iter (fun (x:Db.OrderItemDetails) -> 
        match (canMoveOrderItemToNextStep x user) with
        | true -> Db.tryMoveOrderItemToNextState x.Orderitemid user.UserId ctx
        | false -> ())
    Redirection.FOUND (Path.Orders.myOrders+"#order"+(orderId|> string))

let removeSpooledFiles() =
    log.Debug("removeSpooledFiles()")
    let outFiles = Directory.GetFiles(".") |> Array.toList |> List.filter (fun (x:string ) -> x.EndsWith(".ok"))
    let txtFilesToRemove = outFiles |> List.map (fun x -> x.Replace(".ok",".txt"))
    let _ = outFiles |> List.iter (fun x -> File.Delete(x))
    let _ = txtFilesToRemove |> List.iter (fun x -> File.Delete(x))
    ()


let makeFileOutForAGroupOfordersForDifferentPrinters (orderOutGroupDetail:Db.OrderOutGroupDetail) (plainIngredientsMap:Map<int,string>) (variationsIngredientsMap:Map<int,string>)  =

    log.Debug("makeFileOutForAGroupOfordersForDifferentPrinters")
    let ctx = Db.getContext()
    let _ = removeSpooledFiles()
    let printCount = orderOutGroupDetail.Printcount
    let header = "Tavolo: " + orderOutGroupDetail.Table + "\n"+"Stampa di questa comanda: n. " + (printCount |> string) + "\n"+
        "Gruppo uscita: "+ (orderOutGroupDetail.Groupidentifier |> string) + "\ndata stampa:" + DateTime.Now.ToLocalTime().ToString() + "\n"
    let orderItems = Db.getOrderItemsDetailOfOrderByOutGroup orderOutGroupDetail.Ordergroupid ctx
    let toBeWorkedState = Db.States.getStateByName("TOBEWORKED")
    let printers = Db.getPrinters ctx

    let printersForCategories = printers |> 
        List.map (fun (x:Db.Printer) -> (x,x.``public.printerforcategory by printerid`` |> 
        Seq.map (fun (y:Db.PrinterForCourseCategory) -> y.Categoryid) |> Seq.toList))

    let printersForOrderItems = printersForCategories |> 
        List.map (fun (x,(y: int list)) -> (x, orderItems |> 
            List.filter (fun (z:Db.OrderItemDetails) -> List.contains z.Categoryid  y))) 
            
    let filteredPrintersForOrderItems = printersForOrderItems |> List.filter (fun (_,y) -> ((List.length y) > 0))

    let printersForOrderItemsTextes = filteredPrintersForOrderItems |> 
        List.map (fun (x, (y:Db.OrderItemDetails list)) -> 
        (x, y |> (List.map ( fun (z:Db.OrderItemDetails) ->   
        "n. " + (z.Quantity |> string) + " " + z.Name + " " + "ing. ricetta:" +  (if (plainIngredientsMap.[z.Orderitemid]) = "" then "mancante" else  plainIngredientsMap.[z.Orderitemid])
        + " var: " +  variationsIngredientsMap.[z.Orderitemid] +   z.Comment + "\n")) |> List.fold (+) ""  |> replaceEmojWithPlainText ))

    let filesAndContents = printersForOrderItemsTextes |>  List.map (fun (x:Db.Printer,y:string) -> (x.Name, header + y) )

    let filesNamesAndContents = filesAndContents |> List.map (fun (x,y) ->
        (sprintf "%s%d.txt" x System.DateTime.Now.Ticks,y))

    let filteredFileNamesAndContents = filesNamesAndContents |> List.filter (fun (_,y:string) -> (y.Length > 0))
    let fileNames = filteredFileNamesAndContents |> List.map (fun (x,y) -> x)
    let printerNames = printersForOrderItemsTextes |> List.map (fun (x:Db.Printer, _) -> x.Name)
    let forPrintersFileNames = List.map2 (fun x y -> (x, y) ) printerNames fileNames

    let _ = filteredFileNamesAndContents |> List.iter (fun (x,y) ->
        let outFile = new System.IO.StreamWriter(x,true,Encoding.UTF8)
        let _ = outFile.WriteLine(y)
        let _ = outFile.WriteLine()
        outFile.Close() |> ignore
        )
    let _ = forPrintersFileNames |> List.iter (fun (x,y) -> 
        System.Diagnostics.Process.Start(Settings.Printcommand, "-P " + x + " " + AppDomain.CurrentDomain.BaseDirectory + y) |> ignore
        File.Copy(y,y.Replace(".txt",".ok"))
    )
    ()


let fileDumpOrderOutGroupForDifferentPrinters (orderOutGroup:Db.OrderOutGroupDetail) =
    log.Debug("fileDumpOrderOutGroupForDifferentPrinters")
    let ctx = Db.getContext()
    let orderItemMyRoleCanObserve = Db.getOrderDetailsOfGroupDetail orderOutGroup ctx

    let listOfVariations = orderItemMyRoleCanObserve |> 
        List.map (fun (x:Db.OrderItemDetails) -> (x.Orderitemid,(Db.getVariationDetailsOfOrderItem x.Orderitemid ctx)))  

    let variationsByStringDescription = Utils.variationsByStringDescription listOfVariations ctx

    let mapOfVariationsWithIngredientsIds = listOfVariations |> 
        List.map (fun (id, variations:(Db.VariationDetail list)) -> 
            (id,variations |> List.map (fun (v:Db.VariationDetail) -> v.Ingredientid))) |> Map.ofList

    let orderItemIngredientsMap =  orderItemMyRoleCanObserve |> 
        List.map ( fun (x:Db.OrderItemDetails) ->  
           (x.Orderitemid, (Db.getIngredientsOfACourse x.Courseid ctx) |> List.filter (fun (y:Db.IngredientOfCourse) -> 
           not (List.contains y.Ingredientid mapOfVariationsWithIngredientsIds.[x.Orderitemid] ))))

    let orderItemIngredientsMapSequence = orderItemIngredientsMap |>

        List.map (fun (orderItemId,(ingredients:Db.IngredientOfCourse list)) -> 
            (orderItemId, List.fold (fun y (x:Db.IngredientOfCourse)  ->   
                (if (x.Unitmeasure = Globals.UNITARY_MEASUSERE && (x.Quantity > (decimal)1)) then ((x.Quantity |> int |> string)+" ") else "")+ 
                x.Ingredientname+", " + y  ) "" ingredients)) |>
        Map.ofList

    makeFileOutForAGroupOfordersForDifferentPrinters orderOutGroup orderItemIngredientsMapSequence  variationsByStringDescription




let printOrderOutGroup orderOutGroupId  (order:Db.Order) (orderItemsDetails:Db.OrderItemDetails list) =
    log.Debug("printOrderOutGroup")
    let ctx = Db.getContext()
    let orderOutGroup = Db.getOutGroup orderOutGroupId ctx
    orderOutGroup.Printcount <- orderOutGroup.Printcount + 1
    ctx.SubmitUpdates()
    let orderOutGroupDetail = Db.getOutGroupDetail orderOutGroup.Ordergroupid  ctx
    fileDumpOrderOutGroupForDifferentPrinters orderOutGroupDetail

let rePrintOrderOutGroup orderId orderOutGroupId encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug("rePrintOrderOutGrop")
    let ctx = Db.getContext()
    let decodedBackUrl = WebUtility.UrlDecode encodedBackUrl
    let order = Db.Orders.getOrder orderId ctx
    let orderItemsDetails = Db.getOrderItemsDetailOfOrderByOutGroup orderOutGroupId ctx
    let _ = match (Settings.Print,user.UserId = order.Userid || user.Role = "admin") with
        | (true,true) -> printOrderOutGroup orderOutGroupId order orderItemsDetails
        | _ -> ()
    Redirection.FOUND decodedBackUrl

let twoVariationsAreIdentical (variation1:Db.Variation) (variation2:Db.Variation) =
    log.Debug("twoVariationsAreIdentical")
    variation1.Ingredientid = variation2.Ingredientid &&
    variation1.Tipovariazione = variation2.Tipovariazione &&
    variation1.Orderitemid = variation2.Orderitemid 

let twoListsOfVariationsAreIdentical (variations1:Db.Variation list) (variations2:Db.Variation list) =
    log.Debug("twoListsOfVariationsAreIdentical")
    let orderedVariations1 = variations1 |> List.sortBy (fun (x:Db.Variation) -> x.Ingredientid)
    let orderedVariations2 = variations2 |> List.sortBy (fun (x:Db.Variation) -> x.Ingredientid)
    (List.length orderedVariations1) = (List.length orderedVariations2)  &&
    (List.zip orderedVariations1 orderedVariations2) |> List.map (fun ((x:Db.Variation),(y:Db.Variation)) ->  twoVariationsAreIdentical x y) |> List.fold (&&) true

let twoOrderItemsAreIdentical (orderItem1: Db.OrderItem) (orderItem2: Db.OrderItem) =
    log.Debug("twoOrderItemsAreIdentical")
    let ctx = Db.getContext()
    let initialStateId = (Db.States.getInitState ctx).Stateid
    orderItem1.Stateid = initialStateId && orderItem2.Stateid = initialStateId &&
    not orderItem1.Hasbeenrejected && not orderItem2.Hasbeenrejected  &&
    orderItem1.Orderid = orderItem2.Orderid &&
    orderItem1.Price = orderItem2.Price &&
    orderItem1.Courseid = orderItem2.Courseid &&
    orderItem1.Ordergroupid = orderItem2.Ordergroupid &&
    orderItem1.Comment = orderItem2.Comment &&
    orderItem1.Archived = orderItem2.Archived &&
    orderItem1.Isinsasuborder = orderItem2.Isinsasuborder 

let rec detectIdenticalOrderItemsAndMakeChunks (orderItems:Db.OrderItem list) =
    log.Debug("detectIdenticalOrderItemsAndMakeChunks")
    match orderItems with
    | [] -> []
    | H::T -> [H::(List.filter (fun (x:Db.OrderItem) -> twoOrderItemsAreIdentical x  H) T )] @ detectIdenticalOrderItemsAndMakeChunks ((List.filter (fun (x:Db.OrderItem) -> not (twoOrderItemsAreIdentical x  H)) T))

let moveInitialStateOrderItemsByOrderOutGroup orderId orderOutGroupId  encodedBackUrl (user:UserLoggedOnSession)  =
    log.Debug(sprintf "%s orderId %d orderOutGroupId %d" "moveInitialStateOrderItemsByOrderOutGroup" orderId orderOutGroupId)
    let ctx = Db.getContext()

    let orderItems = Db.getOrderItemsOfOrderByOutGroup orderOutGroupId ctx
    let chunkedOrderItems = detectIdenticalOrderItemsAndMakeChunks orderItems
    let byChunksOptimizableOrderItems = chunkedOrderItems |> List.filter (fun x -> (List.length x> 1))

    let headsOfChunksWithQuantities = byChunksOptimizableOrderItems |> List.map (fun x -> (List.head x, x |> List.fold (fun acc (y:Db.OrderItem) -> y.Quantity + acc) 0 ))
    let tailsOfChunks = byChunksOptimizableOrderItems |> List.map (fun x -> List.tail x)
    let _ = headsOfChunksWithQuantities |> List.iter (fun ((x:Db.OrderItem),count ) -> x.Quantity <- count; ctx.SubmitUpdates()  )
    let _ = tailsOfChunks |> List.iter (fun (x: Db.OrderItem list) -> x |>  List.iter (fun (y:Db.OrderItem) -> Db.safeRemoveOrderItem y.Orderitemid ctx; ctx.SubmitUpdates()))

    let decodedBackUrl = WebUtility.UrlDecode encodedBackUrl
    let order = Db.Orders.getOrder orderId ctx
    let orderItemsDetails = Db.getOrderItemsDetailOfOrderByOutGroup orderOutGroupId ctx
    let orderItemsToMove = orderItemsDetails |> 
        List.filter (fun (x:Db.OrderItemDetails) -> (Db.isInitialState x.Stateid ctx))
    let _ = match orderItemsToMove.Length with
        | X when (X>0 && Settings.Print ) -> printOrderOutGroup orderOutGroupId order orderItemsDetails
        | _ -> ()

    let _ = orderItemsToMove |> Seq.iter (fun (x:Db.OrderItemDetails) -> 
        match (canMoveOrderItemToNextStep x user) with
        | true -> Db.tryMoveOrderItemToNextState x.Orderitemid user.UserId ctx
        | false -> ())
    Redirection.FOUND decodedBackUrl

let adjustTotalOfOrder orderId =
    log.Debug("adjustTotalOfOrder")
    let ctx = Db.getContext()

    let order = Db.Orders.getOrder orderId ctx

    let orderItemsOfTheOrder = Db.Orders.getOrderItemsOfOrderItemThatAreNotInInitalState orderId ctx
    let total = List.fold (fun x  (y:Db.OrderItem) -> x + (y.Price*(decimal)y.Quantity))  ((decimal)0) orderItemsOfTheOrder

    let adjustedTotal = 
        match (order.Adjustispercentage,order.Adjustisplain) with
        |  (true,false) -> total + (order.Percentagevariataion/((decimal)100.0))*total
        |  (false,true) -> total + order.Plaintotalvariation
        | _ -> total

    Db.setTotalOfOrder orderId total ctx
    Db.setAdjstedTotalOfOrder orderId adjustedTotal ctx

let setOrderToArchivedIfAllorderItemsAreDone  orderId =
    log.Debug("setOrderToArchivedIfAllorderItemsAreDone")
    let ctx = Db.getContext()
    let orderItemsOfTheOrder = Db.getAllOrderItemsOfOrder orderId ctx
    let states = Db.States.getOrdinaryStates ctx
    let finalState = states |> List.find  (fun (x:Db.State) -> x.Isfinal)

    let allOrderItemsAreDone = List.forall (fun (x:Db.OrderItem) -> (x.Stateid = finalState.Stateid)) orderItemsOfTheOrder
    let ret =
       match allOrderItemsAreDone with
        | true -> 
            Db.Orders.setOrderAsDoneById orderId ctx
            let total = List.fold (fun x  (y:Db.OrderItem) -> x + (y.Price*(decimal)y.Quantity))  ((decimal)0) orderItemsOfTheOrder
            Db.setTotalOfOrder orderId total ctx
            Db.setAdjstedTotalOfOrder orderId total ctx
        | false -> ()  
    ret

// deprecated
let moveOrderItemNextStep orderItemId returnPath (user:UserLoggedOnSession) =
    log.Debug("moveOrderItemNextStep")
    warbler (fun _ ->
        let ctx = Db.getContext()
        let orderItemDetail = Db.Orders.tryGetOrderItemDetail orderItemId ctx
        match orderItemDetail with
        | Some theOrderItemDetail -> 
            moveOrderItemNextState theOrderItemDetail user
            setOrderToArchivedIfAllorderItemsAreDone  theOrderItemDetail.Orderid
        | None -> () //   failwith ("no order item with id " + orderItemId.ToString())
        
        Redirection.FOUND returnPath
    )

let moveOrderItemNextStepAndBacktoOrder orderItemId encodedBackUrl (user:UserLoggedOnSession) =
    warbler (fun x ->
        let retPath = WebUtility.UrlDecode encodedBackUrl
        let ctx = Db.getContext()
        let orderItemDetail = Db.Orders.tryGetOrderItemDetail orderItemId ctx
        match orderItemDetail with
        | Some theOrderItemDetail -> 
            moveOrderItemNextState theOrderItemDetail user
            Redirection.FOUND retPath
        | None ->   Redirection.FOUND retPath
    )

let removeOrderItemThenGoBackToUrl orderItemId encodedBackUrl (user:UserLoggedOnSession ) =
    warbler (fun x ->
        let retPath = WebUtility.UrlDecode encodedBackUrl
        let ctx = Db.getContext()
        let orderItemDetail = Db.Orders.tryGetOrderItemDetail orderItemId ctx
        match orderItemDetail with
        | Some theOrderItemDetail -> 
            Db.safeRemoveOrderItem orderItemId ctx
            Redirection.FOUND retPath
        | None ->   Redirection.FOUND retPath
    )



let orderItemProgress (user:UserLoggedOnSession) =
    log.Debug("orderItemProgress")
    let ctx = Db.getContext()
    let myRoleObservablesIds = 
        Db.getObserversCatgoryStateMappingForRole user.RoleId ctx |> 
            List.map (fun (x:Db.Observer) -> (x.Stateid, x.Categoryid))
    let mapOfLinkedStates = Db.getStatesNextStatesPairs ctx |> Map.ofList

    let orderItemMyRoleCanObserve = List.map (fun x -> Db.getOrderItemDetailsOfAParticularStateAndAParticularCategory x ctx) myRoleObservablesIds
    let concatenatedOrderItemMyRoleCanObserve = List.fold (fun x y -> x@y) [] orderItemMyRoleCanObserve  |> 
        List.sortBy (fun (x:Db.OrderItemDetails) -> x.Startingtime)

    let listOfVariations = concatenatedOrderItemMyRoleCanObserve |> 
        List.map (fun (x:Db.OrderItemDetails) -> (x.Orderitemid,(Db.getVariationDetailsOfOrderItem x.Orderitemid ctx))) 


    let mapOfVariations = listOfVariations |> Map.ofList

    let mapOfVariationsWithIngredientsIds = listOfVariations |> 
        List.map (fun (id, variations:(Db.VariationDetail list)) -> 
            (id,variations |> List.map (fun (v:Db.VariationDetail) -> v.Ingredientid))) |> Map.ofList

    let orderItemIngredientsMap =  concatenatedOrderItemMyRoleCanObserve |> 
        List.map ( fun (x:Db.OrderItemDetails) ->  
           (x.Orderitemid, (Db.getIngredientsOfACourse x.Courseid ctx) |> List.filter (fun (y:Db.IngredientOfCourse) -> 
           not (List.contains y.Ingredientid mapOfVariationsWithIngredientsIds.[x.Orderitemid] ))))

    let orderItemIngredientsMapSequence = orderItemIngredientsMap |>
        List.map (fun (orderItemId,(ingredients:Db.IngredientOfCourse list)) -> 
            (orderItemId, List.fold (fun y (x:Db.IngredientOfCourse)  -> x.Ingredientname+", " + y  ) "" ingredients)) |>
        Map.ofList

    let variationsStringDescriptions = Utils.variationsByStringDescription (mapOfVariations |> Map.toList) ctx

    html (View.viewableOrderItems concatenatedOrderItemMyRoleCanObserve mapOfLinkedStates mapOfVariations orderItemIngredientsMapSequence variationsStringDescriptions) 

let getNextState stateId =
    log.Debug("getNextState")
    let ctx = Db.getContext()
    let state = Db.States.getState stateId  ctx
    if (state.Isfinal) then state else
        let nextStateId = state.Nextstateid
        Db.States.getState nextStateId ctx

let editOrderItemByCategoryForStrippedUsers (orderItemId:int) categoryId landingPage  encodedBackUrl  (user:UserLoggedOnSession)=
    log.Debug("getNextState")
    let ctx = Db.getContext()
    let orderItem = Db.Orders.tryGetOrderItemDetail orderItemId ctx
    let categories = Db.Courses.getActiveCategories ctx
    let backUrl = WebUtility.UrlDecode(encodedBackUrl)

    match orderItem with
    | Some orderItemExists ->
     let orderId = orderItemExists.Orderid
     let order = Db.Orders.getOrder(orderId) ctx
     let dbUser = Db.getUserById(user.UserId) ctx
     let ingredients = Db.getIngredientsOfCourse orderItemExists.Courseid ctx
     let outGroup = orderItemExists.Groupidentifier

     let viableGroupOutIdsForOrderItem = getViableGroupOutIdentifiers orderId

     if (user.UserId = order.Userid || dbUser.Canmanageallorders || user.Role = "admin") then

     (choose [
        GET >=> warbler (fun x ->
            let courses = 
                Db.Courses.getVisibleCoursesByCategory categoryId  ctx
            html (View.editOrderItemForStrippedUsers orderItemExists courses categories ingredients outGroup backUrl viableGroupOutIdsForOrderItem))

        POST >=> bindToForm Form.strippedOrderItem (fun form ->
            let course = Db.getCourseDetail ((int)form.CourseId) ctx
            let price = course.Price

            Db.updateOrderItemAndPriceByCourseName  orderItemId (form.CourseByName) ((int)(form.Quantity))  form.Comment price form.GroupOut ctx

            makeOrderItemRejectedIfContainsInvisibleIngredients orderItemId ctx
            makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItemId ctx
            Db.deleteAnyEmptyOrderOutGroupOfOrder orderId ctx

            Redirection.FOUND (backUrl+"#order"+(orderItemExists.Orderid |> string))

        )
     ]) else Redirection.FOUND landingPage 

    | None -> Redirection.FOUND landingPage

let editOrderItemByCategoryForOrdinaryUsers (orderItemId:int) categoryId landingPage encodedBackUrl  (user:UserLoggedOnSession)=
    log.Debug("editOrderItemByCategoryForOrdinaryUsers")
    let ctx = Db.getContext()
    let orderItem = Db.Orders.tryGetOrderItemDetail orderItemId ctx
    let categories = Db.Courses.getActiveCategories ctx
    let backUrl = WebUtility.UrlDecode encodedBackUrl

    match orderItem with
    | Some orderItemExists ->
     let orderId = orderItemExists.Orderid
     let order = Db.Orders.getOrder(orderId) ctx
     let dbUser = Db.getUserById(user.UserId) ctx
     let ingredients = Db.getIngredientsOfCourse orderItemExists.Courseid ctx
     let outGroup = orderItemExists.Groupidentifier

     let viableGroupOutIdsForOrderItem = getViableGroupOutIdentifiers orderId

     if (user.UserId = order.Userid || dbUser.Canmanageallorders || user.Role = "admin") then

     (choose [
        GET >=> warbler (fun _ ->
            let courses = 
                Db.Courses.getVisibleCoursesByCategory categoryId  ctx
            html (View.editOrderItemForOrdinaryUsers orderItemExists courses categories ingredients outGroup backUrl viableGroupOutIdsForOrderItem))

        POST >=> bindToForm Form.orderItem (fun form ->
            Db.updateOrderItemAndPriceByCourseName  orderItemId ((form.CourseByName)) ((int)(form.Quantity)) form.Comment form.Price form.GroupOut ctx
            makeOrderItemRejectedIfContainsInvisibleIngredients orderItemId ctx
            makeOrderItemAsRejectedIfContainsUnavalableIngredients orderItemId  ctx
            Db.deleteAnyEmptyOrderOutGroupOfOrder orderId ctx
            Redirection.FOUND (backUrl+"#order"+(orderItemExists.Orderid |> string))
        )
     ]) else Redirection.FOUND landingPage 

    | None -> Redirection.FOUND landingPage

let editOrderItemByCategoryPassingUserLoggedOn (orderItemId:int) categoryId landingPage (encodedBackUrl:string)  (user:UserLoggedOnSession)=
    log.Debug("editOrderItemByCategoryPassingUserLoggedOn")
    let ctx = Db.getContext()
    let dbUser = Db.getUserById user.UserId ctx
    match dbUser.Canchangetheprice with 
    | true -> editOrderItemByCategoryForOrdinaryUsers  (orderItemId:int) categoryId landingPage encodedBackUrl  (user:UserLoggedOnSession)
    | false -> editOrderItemByCategoryForStrippedUsers  (orderItemId:int) categoryId landingPage encodedBackUrl   (user:UserLoggedOnSession)

let resetVariationThenEditOrderItemByCat (orderItemId:int) categoryId landingPage urlEncodedBackUrl (user:UserLoggedOnSession) =
    log.Debug("resetVariationThenEditOrderItemByCat")
    let ctx = Db.getContext()
    let _ = Db.removeAllVariationsOfOrderItem orderItemId ctx
    editOrderItemByCategoryPassingUserLoggedOn orderItemId categoryId landingPage urlEncodedBackUrl user

let createRole (user:UserLoggedOnSession ) =
    log.Debug("createRole")
    let ctx = Db.getContext() 
    choose [
        GET >=> html (View.createRole user)
        POST >=> bindToForm Form.role (fun form ->
            Db.createRole form.Name form.Comment ctx 
            Redirection.FOUND Path.home
         )
    ]

let createOrderByUserLoggedOn (user:UserLoggedOnSession) =
    log.Debug("createOrderByUserLoggedOn")
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            html (View.createOrder "")
        )
        POST >=> bindToForm Form.order (fun form ->
            let thePerson = match form.Person with | Some X -> X | _ -> ""
            let _ = Db.createOrderByUser form.Table thePerson user.UserId ctx
            Redirection.FOUND Path.Orders.myOrders
        )
    ]

let createSingleOrderByUserLoggedOn (user:UserLoggedOnSession) =
    log.Debug("createSingleOrderByUserLoggedOn")
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            html (View.createOrder "")
        )
        POST >=> bindToForm Form.order (fun form ->
            let orderAlreadyExists = Db.tableIsAlreadyInAnOpenOrder form.Table ctx
            match orderAlreadyExists with
                | true -> 
                    View.createOrder "esiste gia' un ordine associato a quel tavolo " |> html
                | false ->
                    let thePerson = match form.Person with | Some X -> X | _ -> ""
                    let order = Db.createOrderByUser form.Table thePerson user.UserId ctx
                    Redirection.FOUND (sprintf Path.Orders.viewOrder order.Orderid)
        )
    ]


type myModel = {name: string; roleidname: indexNameRecord list; categoriesidname: indexNameRecord list;
    existingrolestatemapping:  twoindexnamerecord list; selectroleid: string; selectcategoryid: string;
    backurl: string}

let roleEnablerObserverCategoriesByCheckBoxesWithRoleAndCat roleId categoryId =
    log.Debug("roleEnablerObserverCategoriesByCheckBoxesWithRoleAndCat")
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ ->
            let roleIdNameMap =  Db.getRoles ctx |>   List.map (fun (g:Db.Role) -> g.Roleid,g.Rolename)
            let roleIdNameMapRef =  Db.getRoles ctx |>   List.map (fun (g:Db.Role) -> g.Roleid,g.Rolename) |> Map.ofList
            let statesIdNameMapRef = Db.States.getOrdinaryStates ctx |> List.map (fun (g:Db.State) -> g.Stateid,g.Statusname) |> Map.ofList
            let categoriesIdNameMap = Db.Courses.getActiveCategories ctx |> List.map (fun (g:Db.CourseCategories) -> g.Categoryid,g.Name)
            let categoriesIdNameMapRef = Db.Courses.getActiveCategories ctx |> List.map (fun (g:Db.CourseCategories) -> g.Categoryid,g.Name) |> Map.ofList
            let roleIdsRef = roleIdNameMapRef |> Map.toSeq |> Seq.map (fun (id,_) -> id)
            let statesIdsRef = statesIdNameMapRef |> Map.toSeq |> Seq.map (fun (id,_) -> id)  |> Seq.toList
            let categoriesIdsRef = categoriesIdNameMapRef |> Map.toSeq |> Seq.map (fun (id,_) -> id) |> Seq.toList

            let rolecategoriesCombinations = [for i in roleIdsRef do for j in categoriesIdsRef do 
                 let observers = List.fold (fun acc k ->  (if (Db.isObserverRoleCatState i j k ctx) then ({entry="Observer"+(statesIdNameMapRef.[k])})::acc else acc)) [] statesIdsRef
                 let enablers = List.fold (fun acc k ->  (if (Db.isEnablerRoleCatState i j k ctx) then ({entry="Enabler"+(statesIdNameMapRef.[k])})::acc else acc)) [] statesIdsRef
                 yield {index1=i;index2=j;enablers = enablers; observers = observers}
            ]

            let o = {name = "15";
             roleidname = roleIdNameMap|> List.map (fun (ind,value)-> {index=ind;name=value});
             categoriesidname = categoriesIdNameMap |> List.map (fun (ind,value) -> {index=ind;name=value});
             existingrolestatemapping=rolecategoriesCombinations; selectroleid = (roleId|>string) ; selectcategoryid = (categoryId |> string);
             backurl = Path.Admin.roles
            }

            DotLiquid.page("rolesCategoryMappings.html")  o
        )
        POST >=> bindToForm Form.enablersObserverForStateAbilities (fun form -> 
            let _ =  
                match form.EnablerCOLLECTING with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryEnablerByStateName ((int)form.RoleId) "COLLECTING" ((int)form.CategoryId) ctx 
                    ()
                | None -> 
                    let _ = Db.tryRemoveRoleStateCategoryEnablerByStateName ((int)form.RoleId) "COLLECTING" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.EnablerTOBEWORKED with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryEnablerByStateName ((int)form.RoleId) "TOBEWORKED" ((int)form.CategoryId) ctx
                    ()
                | None -> 
                    let _ = Db.tryRemoveRoleStateCategoryEnablerByStateName ((int)form.RoleId) "TOBEWORKED" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.EnablerSTARTEDWORKING with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryEnablerByStateName ((int)form.RoleId) "STARTEDWORKING" ((int)form.CategoryId) ctx
                    ()
                | None -> 
                    let _ = Db.tryRemoveRoleStateCategoryEnablerByStateName ((int)form.RoleId) "STARTEDWORKING" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.EnablerREADYFORDELIVERY with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryEnablerByStateName ((int)form.RoleId) "READYFORDELIVERY" ((int)form.CategoryId) ctx
                    ()
                | None -> 
                    let _ = Db.tryRemoveRoleStateCategoryEnablerByStateName ((int)form.RoleId) "READYFORDELIVERY" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.EnablerDELIVERED with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryEnablerByStateName ((int)form.RoleId) "DELIVERED" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryEnablerByStateName ((int)form.RoleId) "DELIVERED" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.EnablerDONE with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryEnablerByStateName ((int)form.RoleId) "DONE" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryEnablerByStateName ((int)form.RoleId) "DONE" ((int)form.CategoryId) ctx
                    ()


            let _ = 
                match form.ObserverCOLLECTING with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "COLLECTING" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "COLLECTING" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.ObserverTOBEWORKED with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "TOBEWORKED" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "TOBEWORKED" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.ObserverTOBEWORKED with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "TOBEWORKED" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "TOBEWORKED" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.ObserverSTARTEDWORKING with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "STARTEDWORKING" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "STARTEDWORKING" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.ObserverREADYFORDELIVERY with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "READYFORDELIVERY" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "READYFORDELIVERY" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.ObserverDELIVERED with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "DELIVERED" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "DELIVERED" ((int)form.CategoryId) ctx
                    ()
            let _ = 
                match form.ObserverDONE with
                | Some X -> 
                    let _ = Db.tryCreateRoleStateCategoryObserverByStateName ((int)form.RoleId) "DONE" ((int)form.CategoryId) ctx
                    ()
                | _ -> 
                    let _ = Db.tryRemoveRoleStateCategoryObserverByStateName ((int)form.RoleId) "DONE" ((int)form.CategoryId) ctx
                    ()

            Redirection.found (sprintf Path.Admin.roleEnablerObserverCategoriesByCheckBoxesByRoleAndCat ((int)form.RoleId) ((int)form.CategoryId))
        )
        
    ]

let roleEnablerObserverCategoriesByCheckBoxes   =
    log.Debug("roleEnablerObserverCategoriesByCheckBoxes")
    let ctx = Db.getContext()

    let firstRole = Db.getAllRoles ctx |> List.tryHead

    let firstRoleId = 
        match firstRole with
        | Some X -> X.Roleid
        | None -> -1

    let firstCategory  = Db.Courses.getAllourseCategories ctx |> List.tryHead

    let firstCategoryId = 
        match firstCategory with
        | Some X -> X.Categoryid
        | None -> -1

    roleEnablerObserverCategoriesByCheckBoxesWithRoleAndCat firstRoleId firstCategoryId 



type model =  { names: indexNameRecord list; measures: indexUnitMeasureMap list; message: string}

let selectIAllngredientCatForCourse courseId  =

    let ctx = Db.getContext()
    choose [
    GET >=> warbler (fun _ ->
        log.Debug("selectIAllngredientCatForCourse GET")
        let visibleIngredients = Db.getAllVisibleIngredients  ctx
        let alreadyTakenIngredients = Db.getIngredientIdsOfACourse courseId ctx
        let selectableIngredients = visibleIngredients |> List.filter (fun (x:Db.Ingredient) -> not (List.contains x.Ingredientid alreadyTakenIngredients))
        let myMap = selectableIngredients |> List.map (fun (x:Db.Ingredient) -> {index=x.Ingredientid;name=x.Name })

        let indexUnitsOfMeasures = List.map (fun (x:Db.Ingredient) -> {index=x.Ingredientid ; unitmeasure = x.Unitmeasure}) selectableIngredients

        let o = {names = myMap; measures=indexUnitsOfMeasures; message = ""}
        let course = Db.Courses.tryGetCourse courseId ctx
        match course with
        | Some theCourse ->
            DotLiquid.page("ingredientToCourse.html") o
        | _ ->  Redirection.FOUND Path.home
    ) 
    POST >=> bindToForm Form.ingredientSelector (fun form ->
        log.Debug("selectIAllngredientCatForCourse POST")
        let decQuantity = form.Quantity 
        Db.addIngredientToCourse ((int)form.IngredientBySelect) courseId decQuantity ctx

        let retPath = sprintf Path.Courses.editCourse courseId 
        Redirection.FOUND retPath
    )
 ]



let selectIngredientCatForCourse courseId categoryId message =
    let ctx = Db.getContext()
    choose [

        GET >=> warbler (fun _ ->
            log.Debug("selectIngredientCatForCourse GET")
            let visibleIngredients = Db.getVisibleIngredientsOfACategory categoryId ctx
            let alreadyTakenIngredients = Db.getIngredientIdsOfACourse courseId ctx
            let selectableIngredients = visibleIngredients |> 
                List.filter (fun (x:Db.Ingredient) -> not (List.contains x.Ingredientid alreadyTakenIngredients))
            let myMap = selectableIngredients |> List.map (fun (x:Db.Ingredient)   -> {index=x.Ingredientid;name=x.Name })
            let indexUnitsOfMeasures = List.map (fun (x:Db.Ingredient) -> {index=x.Ingredientid ; unitmeasure = x.Unitmeasure}) selectableIngredients

            let o = { names = myMap; measures=indexUnitsOfMeasures; message = ""}
            DotLiquid.page ("ingredientToCourse.html") o 
        )
        POST >=> bindToForm Form.ingredientSelector (fun form ->
            log.Debug("selectIngredientCatForCourse POST")
            let ingredient = Db.tryGetIngredientByName form.IngredientByText ctx
            let _ = match ingredient with 
                | Some theIngredient -> 
                    let quantity = match (form.Quantity,theIngredient.Unitmeasure) with
                                | (Some X,_) -> Some X
                                | (None,Globals.UNITARY_MEASUSERE) -> Some ((decimal)1.0)
                                | _ -> form.Quantity

                    Db.addIngredientToCourseByName (form.IngredientByText) courseId quantity ctx
                | None -> ()

            let retPath = sprintf Path.Courses.editCourse courseId 
            Redirection.FOUND retPath
        )
    ]


let createCourseByCatgory id =
    log.Debug("createCourseByCatgory")
    let ctx = Db.getContext()
    choose [
        GET >=> warbler (fun _ -> 
            let visibleCategories = Db.Courses.getVisibleCourseCategories ctx |> List.map (fun (g:Db.CourseCategories) -> (decimal)g.Categoryid,g.Name)
            html (View.createCourseByCategory "" visibleCategories id))
        POST >=> bindToForm Form.course (fun form ->
            let visibleCategories = Db.Courses.getVisibleCourseCategories ctx |> List.map (fun (g:Db.CourseCategories) -> (decimal)g.Categoryid,g.Name)

            match Db.Courses.tryFindCourseByName form.Name ctx with
            | Some _ -> 
                View.createCourseByCategory (local.TheName+ form.Name + "esiste già") visibleCategories id |> html

            | None ->
                let description = match form.Description with | Some X -> X | _ -> ""
                let course = Db.Courses.createCourse form.Price form.Name description (form.Visibile = "VISIBLE") ((int)form.CategoryId) ctx
                let retPath = sprintf Path.Courses.editCourse course.Courseid 
                Redirection.FOUND retPath
        )
    ]



let archiveOrder id (user:UserLoggedOnSession) =
    let ctx = Db.getContext()
    let dbUser = Db.getUserById (user.UserId) ctx
    let _ = if (dbUser.Canmanageallorders || user.Role = "admin") then
                Db.archiveOrder id ctx
                Db.pushArchivedOrdersInLog id ctx
            else ()
    Redirection.FOUND Path.Orders.seeDoneOrders



let unableToCompleteOperation aString    =
    View.cantComplete aString |> html

let voidorder orderId = 
    let ctx = Db.getContext()
    Db.voidOrder orderId ctx
    Redirection.FOUND Path.home




let voidOrderByUserLoggedOn orderId urlEncodedBackUrl (user: UserLoggedOnSession) =
    log.Debug("voidOrderByUserLoggedOn")
    let ctx = Db.getContext()
    let order = Db.Orders.tryGetOrder(orderId) ctx
    let dbUser = Db.getUserById(user.UserId) ctx
    let backUrl = WebUtility.UrlDecode urlEncodedBackUrl
    match order with 
    | Some theOrder -> warbler (fun x ->
        if (user.Role = "admin" || (theOrder.Userid = user.UserId && dbUser.Canvoidorders))
            then 
                let _ = Db.voidOrder orderId ctx
                let _ = Db.addVoidedOrderToLog orderId user.UserId ctx
                Redirection.FOUND backUrl
        else 
                Redirection.FOUND backUrl)
    | _ -> Redirection.FOUND Path.home 
    

let askConfirmationVoidOrderByUserLoggedOn orderId encodedBackUrl (user: UserLoggedOnSession) =
    log.Debug(sprintf "%s order: %d  user: %s" "askConfirmationVoidOrderByUserLoggedOn" orderId user.Username)
    View.askConfirmationVoidOrderByUserLoggedOn orderId encodedBackUrl (user: UserLoggedOnSession) |> html






let deleteIngredientToCourse courseId ingredientId =
    log.Debug("deleteIngredientToCourse")
    let ctx = Db.getContext()
    let ingredientcourse = Db.tryGetIngredientCourse courseId ingredientId ctx
    match ingredientcourse with 
    | Some theIngredientCourse -> theIngredientCourse.Delete(); ctx.SubmitUpdates()
    | None -> ()
    let retPath = sprintf Path.Courses.editCourse courseId 
    Redirection.FOUND retPath


let allSubOrdersOfOrderDetailBelongsToAPaidSubOrder orderId =
    let ctx = Db.getContext()
    let orderItems = Db.Orders.getOrderItemsOfOrder orderId ctx
    let orderItemsBelongingToSubOrder = orderItems |> List.filter(fun (x:Db.OrderItem) -> x.Isinsasuborder)
    (List.length orderItemsBelongingToSubOrder) = (List.length orderItems)

let archiveOrderByUserId orderId  =
    let ctx = Db.getContext()
    let order = Db.Orders.getOrder orderId ctx
    match order.Archived with
         | true -> ()
         | false -> Db.archiveOrder orderId ctx
                    Db.pushArchivedOrdersInLog orderId ctx
                    ()

let archiveOrdersWithAllOrderItemsInPaidSubOrders (orders: (Db.NonArchivedOrderDetail) list)  =
    let paidOrders = orders |> List.filter (fun order -> allSubOrdersOfOrderDetailBelongsToAPaidSubOrder order.Orderid )
    let _ = paidOrders |> List.iter (fun x -> archiveOrderByUserId x.Orderid  )
    ()


let seeDoneOrders (user:UserLoggedOnSession)=
    log.Debug("seeDoneOrders")
    let ctx  = Db.getContext()

    let loadedOrders = Db.Orders.getPayableOrderDetails ctx  

    let _ = loadedOrders |> List.iter (fun (x:Db.NonArchivedOrderDetail) -> adjustTotalOfOrder x.Orderid )
    let orders = Db.Orders.getPayableOrderDetails ctx  

    let orderItemsOfOrders = orders |> List.map (fun (x:Db.NonArchivedOrderDetail) -> 
        (x.Orderid, Db.getOrderItemDetailOfOrderDetailThatAreNotInInitStateByNonEmptyOrderDetail x ctx)) |> Map.ofList 

    let dbUser = Db.getUserById(user.UserId) ctx

    let subOrdersOfOrdersMap = orders |> 
        List.map (fun (x:Db.NonArchivedOrderDetail) -> (x.Orderid,Db.Orders.getSubOrdersOfOrderById x.Orderid ctx)) 

    let ordersHavingSubOrdersMap = subOrdersOfOrdersMap |> List.map (fun (x,y) -> (x,(List.length y >0)))  |> Map.ofList

    match dbUser.Canmanageallorders with
    | true -> 
        View.seeDoneOrders orders orderItemsOfOrders ordersHavingSubOrdersMap |> html
    | _ -> Redirection.found Path.home

let adjustPriceOfOrderItemByVariations orderItemId =
    log.Debug(sprintf "%s %d" "adjustPriceOfOrderItemByVariations" orderItemId)
    let ctx = Db.getContext()
    let orderItem = Db.getTheOrderItemById orderItemId ctx

    let course = Db.Courses.getCourse orderItem.Courseid ctx
    let originalPrice = course.Price

    let variations = orderItem.``public.variations by orderitemid`` |> Seq.toList
    let addVariations = variations |> List.filter (fun (x:Db.Variation) -> x.Tipovariazione = Globals.AGGIUNGIMOLTO || 
            x.Tipovariazione = Globals.AGGIUNGINORMALE || x.Tipovariazione = Globals.AGGIUNGIPOCO)

    let subtractVariations = variations |> List.filter (fun (x:Db.Variation) -> x.Tipovariazione = Globals.SENZA) 

    let unitaryAddOrSubtractVariations = variations |> List.filter (fun (x:Db.Variation) -> x.Tipovariazione = Globals.UNITARY_MEASUSERE)
    let unitaryAddOrSubtract = unitaryAddOrSubtractVariations |> List.map (fun (x:Db.Variation) -> ((decimal)x.Plailnumvariation) * (Db.getFirstPriceVariationForIngredientAddVariatonFlaggedAsDefault x.Ingredientid ctx )) |>
        List.fold (+) ((decimal)0.0)

    let specificIngredientPriceBasedVariations = variations |> List.filter (fun (x:Db.Variation) -> x.Tipovariazione = Globals.PER_PREZZO_INGREDIENTE)

    let amountToAddToPriceBeacuseOfAddings = match Settings.InAddIngredientAdjustPrice with 
                                            | true ->  addVariations |> List.map (fun (x:Db.Variation) -> 
                                                Db.getFirstPriceVariationForIngredientAddVariatonFlaggedAsDefault x.Ingredientid ctx) |> List.fold (+) ((decimal)0.0)
                                            | false -> (decimal)0.0

    let amountToSubtractToPriceBeacuseOfSubtractions = match Settings.InAddIngredientAdjustPrice with 
                                            | true ->  subtractVariations |> List.map (fun (x:Db.Variation) -> 
                                                Db.getFirstPriceVariationForIngredientSubtractVariatonFlaggedAsDefault x.Ingredientid ctx) |> List.fold (+) ((decimal)0.0)
                                            | false -> (decimal)0.0

    let pricesGivenToTheIngredientPrices = specificIngredientPriceBasedVariations |> List.map (fun (x:Db.Variation) -> (Db.getIngredientPrice x.Ingredientpriceid ctx)) |> 
        List.map (fun (x:Db.IngredientPrice) -> x.Addprice ) |> List.fold (+) ((decimal)0)

    let _ = orderItem.Price <- originalPrice + amountToAddToPriceBeacuseOfAddings - amountToSubtractToPriceBeacuseOfSubtractions + unitaryAddOrSubtract + pricesGivenToTheIngredientPrices

    ctx.SubmitUpdates()



let editOrderItemVariationPassingUserLoggedOnAndIngredientList orderItemId (ingredientsYouCanAdd: Db.IngredientDetail list) encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d " "editOrderItemVariationPassingUserLoggedOnAndIngredientList" orderItemId)
    let ctx = Db.getContext()
    let orderItemDetail = Db.Orders.tryGetOrderItemDetail orderItemId ctx
    let ingredientCategories = Db.getVisibleIngredientCategories ctx
    match orderItemDetail with
    | Some theOrderItemDetail -> 
        let dbUser = Db.getUserById user.UserId ctx
        if (user.UserId = theOrderItemDetail.Userid || dbUser.Canmanageallorders || user.Role = "admin" ) then
          choose [
            GET >=> warbler (fun _ ->
                let ctx = Db.getContext()
                let _ = adjustPriceOfOrderItemByVariations theOrderItemDetail.Orderitemid

                let specificCustomAddQuantitiesForIngredients = ingredientsYouCanAdd |> List.map (fun (x:Db.IngredientDetail) -> (x.Ingredientid,Db.getIngredientPrices x.Ingredientid ctx)) |> Map.ofList

                (let ingredientOfCourse = Db.getIngredientsOfCourse theOrderItemDetail.Courseid ctx
                 let existingVariations =  Db.getAllVariationDetailsOfOrderItem theOrderItemDetail.Orderitemid ctx


                 html (View.editOrderItemVariations theOrderItemDetail ingredientOfCourse 
                    existingVariations ingredientCategories ingredientsYouCanAdd specificCustomAddQuantitiesForIngredients  encodedBackUrl  )))

            POST >=> bindToForm Form.addIngredient (fun form -> 
                let ctx = Db.getContext()
                let _ = Db.addAddIngredientVariationByName orderItemId (form.IngredientByText) form.Quantity ctx
                let _ = adjustPriceOfOrderItemByVariations orderItemId
                let redirTo = (sprintf Path.Orders.editOrderItemVariation orderItemId encodedBackUrl)
                Redirection.found (redirTo)
            )
         ]
         
         else (Redirection.FOUND Path.home)
    | None -> Redirection.FOUND Path.home

let viewSingleOrder orderId (user:UserLoggedOnSession)=
    log.Debug(sprintf "%s %d" "viewSingleOrder" orderId)
    let ctx = Db.getContext()
    Db.deleteAnyEmptyOrderOutGroupOfOrder orderId ctx

    let orderDetail = Db.getOrderDetail orderId ctx
    let myOrdersDetails = Db.Orders.getOngoingOrderDetailsByUser user.UserId ctx // orderDetail.Userid  ctx
    let othersOrdersDetails = match user.CanManageAllOrders with
                                | true -> Db.Orders.getOngoingOrderDetailsByAllUserExcept user.UserId ctx // orderDetail.Userid ctx
                                | _ -> []

    let userView = Db.getUserViewById user.UserId ctx //orderDetail.Userid ctx
    let activeCategories = Db.Courses.getActiveConcreteCategories ctx
    let orderItemDetails = Db.getOrderItemDetailOfOrderById orderId ctx
    let mapOfLinkedStates = Db.getMapOfStates ctx 
    let statesEnabledForUser = Db.listOfEnabledStatesForWaiter userView.Userid ctx
    let eventualRejectionsOfOrderItems = [ orderDetail ] |>
         List.map (fun (x:Db.Orderdetail) ->  Db.getOrderItemDetailOfOrderDetail x ctx  ) |> List.fold (@) [] 
         |> List.map (fun (x:Db.OrderItemDetails) -> (x.Orderitemid,Db.getLatestRejectionOfOrderItem x.Orderitemid ctx)) |> Map.ofList

    let outGroupsOfOrder = Db.getOutGroupsOfOrder orderId ctx

    (View.viewSingleOrder orderDetail orderItemDetails mapOfLinkedStates statesEnabledForUser 
        eventualRejectionsOfOrderItems activeCategories userView outGroupsOfOrder myOrdersDetails othersOrdersDetails user) |> html




let editOrderItemVariationPassingUserLoggedOn orderItemId encodedBackUrl (user:UserLoggedOnSession) =
    warbler (fun  x ->  
        log.Debug(sprintf "%s %d" "editOrderItemVariationPassingUserLoggedOn" orderItemId )
        let ctx = Db.getContext()
        let theOrderItem = Db.getTheOrderItemById orderItemId ctx
        let _ = theOrderItem.Hasbeenrejected <- false
        let _ = adjustTotalOfOrder theOrderItem.Orderid
        ctx.SubmitUpdates()

        let courseOfOrderItem = Db.Courses.getCourse theOrderItem.Courseid ctx

        let ingredientIdsOfCourse = courseOfOrderItem.``public.ingredientcourse by courseid`` |> Seq.toList |> List.map (fun (x:Db.IngredientCourse) -> x.Ingredientid) 

        let ingredientDetailsYouCanAdd = Db.getAllVisibleIngredientDetails ctx

        let ingredientDetailsYouCanAddWithoutAlreadyThere =  ingredientDetailsYouCanAdd |> 
            List.filter (fun (x:Db.IngredientDetail) -> not (List.contains x.Ingredientid  ingredientIdsOfCourse ))

        editOrderItemVariationPassingUserLoggedOnAndIngredientList orderItemId ingredientDetailsYouCanAddWithoutAlreadyThere encodedBackUrl user 
    )


let editOrderItemVariationByIngredientCategoryPasssingUserLoggedOn orderItemId categoryId encodedBackUrl (user:UserLoggedOnSession) =
    warbler ( fun x ->
        log.Debug(sprintf "%s %d " "editOrderItemVariationByIngredientCategoryPasssingUserLoggedOn" orderItemId)
        let ctx = Db.getContext()

        let ingredientsDetailYouCanAdd = Db.getVisibleIngredientsDetailOfACategory categoryId ctx

        let orderItem = Db.getTheOrderItemById orderItemId ctx

        let courseOfOrderItem = Db.Courses.getCourse orderItem.Courseid ctx
        let ingredientIdsOfCourse = courseOfOrderItem.``public.ingredientcourse by courseid`` |> Seq.toList |> List.map (fun (x:Db.IngredientCourse) -> x.Ingredientid) 

        let ingredientDetailsYouCanAddWithoutAlreadyThere =  ingredientsDetailYouCanAdd |> 
            List.filter (fun (x:Db.IngredientDetail) -> not (List.contains x.Ingredientid  ingredientIdsOfCourse ))

        editOrderItemVariationPassingUserLoggedOnAndIngredientList orderItemId ingredientDetailsYouCanAddWithoutAlreadyThere encodedBackUrl user 
    )

let addWithoutIngredientVariation orderItemId ingredientId encodedBackUrl  (user: UserLoggedOnSession)=
    log.Debug(sprintf "%s %d %d " "addWithoutIngredientVariation" orderItemId ingredientId)
    let ctx = Db.getContext()
    let orderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx

    let course = Db.Courses.getCourse orderItemDetail.Courseid ctx

    let ingredientsMap = Db.getIngredientsOfACourse course.Courseid ctx |> 
        List.map (fun (x:Db.IngredientOfCourse) -> (x.Ingredientid,x)) |> Map.ofList

    let dbUser = Db.getUserById user.UserId ctx

    let _ = if (orderItemDetail.Userid = user.UserId || user.Role = "admin" || dbUser.Canmanageallorders) then 
                (match ingredientsMap.[ingredientId].Unitmeasure with
                | Globals.UNITARY_MEASUSERE  -> Db.addRemoveUnitaryIngredientVariationOrDecreaseByOne orderItemId ingredientId ctx
                | _ -> (Db.addRemoveIngredientVariation orderItemId ingredientId ctx)) 
            else 
            ()

    let _ = adjustPriceOfOrderItemByVariations orderItemId 
    editOrderItemVariationPassingUserLoggedOn orderItemId encodedBackUrl user

let removeAllInvisibleIngredients orderItemId encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d " "removeAllInvisibleIngredients" orderItemId)
    let ctx = Db.getContext()
    let orderItem = Db.Orders.getOrderItemById orderItemId ctx
    match orderItem with
    | Some theOrderItem ->    
        let course = Db.Courses.getCourse theOrderItem.Courseid ctx   
        let ingredientsOfTheCourse = Db.getIngredientsOfCourse course.Courseid ctx
        let unavailables = ingredientsOfTheCourse |> List.filter (fun (x:Db.IngredientOfCourse) -> ( not x.Visibility))
        unavailables |> List.iter (fun (x:Db.IngredientOfCourse) -> Db.addRemoveIngredientVariation orderItemId x.Ingredientid ctx) 
    | _ -> ()
    editOrderItemVariationPassingUserLoggedOn orderItemId encodedBackUrl user

let removeAllAllergenic orderItemId  encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d" "removeAllAllergenic" orderItemId)
    let ctx = Db.getContext()
    let orderItem = Db.Orders.getOrderItemById orderItemId ctx
    match orderItem with 
    | Some theOrderItem ->
        let course = Db.Courses.getCourse theOrderItem.Courseid ctx
        let ingredientsOfTheCourse = Db.getIngredientsOfCourse course.Courseid ctx
        let allergenics = ingredientsOfTheCourse |> List.filter (fun (x:Db.IngredientOfCourse) -> x.Allergen)
        allergenics |> List.iter (fun (x:Db.IngredientOfCourse) -> Db.addRemoveIngredientVariation orderItemId x.Ingredientid ctx)
    | _ -> ()
    editOrderItemVariationPassingUserLoggedOn orderItemId encodedBackUrl user
let addDiminuishIngredientVariation orderItemId ingredientId encodedBackUrl (user:UserLoggedOnSession)=
    log.Debug(sprintf "%s %d %d " "addDiminuishIngredientVariation" orderItemId ingredientId )
    let ctx = Db.getContext()
    let orderDetail = Db.Orders.getOrderItemDetail orderItemId ctx
    let dbUser = Db.getUserById user.UserId ctx
    let _ = if (orderDetail.Userid = user.UserId || user.Role = "admin" || dbUser.Canmanageallorders) then 
        (Db.addDiminuishIngredientVariattion orderItemId ingredientId ctx) else ()
    editOrderItemVariationPassingUserLoggedOn orderItemId encodedBackUrl user

let addIncreaseIngredientVariation orderItemId ingredientId encodedBackUrl (user:UserLoggedOnSession)=
    log.Debug(sprintf "%s %d %d" "addIncreaseIngredientVariation"  orderItemId  ingredientId)
    let ctx = Db.getContext()
    let orderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx
    let course = Db.Courses.getCourse orderItemDetail.Courseid ctx

    let ingredientsMap = Db.getIngredientsOfACourse course.Courseid ctx |> 
        List.map (fun (x:Db.IngredientOfCourse) -> (x.Ingredientid,x)) |> Map.ofList

    let dbUser = Db.getUserById user.UserId ctx
    let _ = if (orderItemDetail.Userid = user.UserId || user.Role = "admin" || dbUser.Canmanageallorders) then 
              (match ingredientsMap.[ingredientId].Unitmeasure with
              | Globals.UNITARY_MEASUSERE -> Db.addAddUnitaryIngredientVariationOrIncreaseByOne orderItemId ingredientId ctx
              | _ -> (Db.addIncreaseIngredientVariation orderItemId ingredientId ctx) )
            else ()

    let _ = adjustPriceOfOrderItemByVariations orderItemId 
    editOrderItemVariationPassingUserLoggedOn orderItemId encodedBackUrl user

let removeIngredientVariation variationId orderitemId  encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d %d " "removeIngredientVariation" variationId orderitemId)
    let ctx = Db.getContext()
    let orderDetail = Db.Orders.getOrderItemDetail orderitemId ctx
    let dbUser = Db.getUserById user.UserId ctx
    let _ = if (orderDetail.Userid = user.UserId || user.Role = "admin" || dbUser.Canmanageallorders) then 
      (Db.removeIngredientVariation  variationId ctx) else ()
    let _ = adjustPriceOfOrderItemByVariations orderitemId
    editOrderItemVariationPassingUserLoggedOn orderitemId encodedBackUrl user

let increaseUnitaryIngredientVariation variationId encodedBackUrl (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d " "increaseUnitaryIngredientVariation" variationId)
    let ctx = Db.getContext()
    let variation = Db.getVariation variationId ctx
    let variationDetail = Db.getVariationDetail variationId ctx
    let dbUser = Db.getUserById user.UserId ctx
    let _ = if (variationDetail.Userid = user.UserId || user.Role = "admin" || dbUser.Canmanageallorders) then
        variation.Plailnumvariation <- variation.Plailnumvariation + 1
        ctx.SubmitUpdates()
    let _ = adjustPriceOfOrderItemByVariations variation.Orderitemid
    editOrderItemVariationPassingUserLoggedOn variation.Orderitemid encodedBackUrl user

let decreaseUnitaryIngredientVariation variationId encodedBackUrl (user:UserLoggedOnSession) =
    let ctx = Db.getContext()
    let variation = Db.getVariation variationId ctx
    let variationDetail = Db.getVariationDetail variationId ctx

    let dbUser = Db.getUserById user.UserId ctx

    let _ = if (variationDetail.Userid = user.UserId || user.Role = "admin" || dbUser.Canmanageallorders) then
        variation.Plailnumvariation <- variation.Plailnumvariation - 1
        ctx.SubmitUpdates()

    let _ = adjustPriceOfOrderItemByVariations variation.Orderitemid

    editOrderItemVariationPassingUserLoggedOn variation.Orderitemid encodedBackUrl user


let deleteUser id  = warbler (fun x ->
    let backUrl = match x.request.queryParam("backUrl") with | Choice1Of2 par -> par | _ -> Path.Admin.deleteObjects
    let ctx = Db.getContext()
    let user = Db.getUserViewById id ctx
    match user.Rolename with
    | "admin" -> Redirection.found backUrl // Path.Admin.deleteObjects
    | _ ->
         Db.safeDeleteUser id ctx
         Redirection.found backUrl // Path.Admin.deleteObjects
    )


let deleteCourseCategory id =
    let ctx =  Db.getContext()
    Db.safeDeleteCourseCategory id ctx
    Redirection.found Path.Admin.deleteObjects

let deleteIngredientCategory id = 
    let ctx = Db.getContext()
    Db.safeDeleteIngredientCategory id ctx
    Redirection.found Path.Admin.deleteObjects

type LiquidWrappedOrderItemsForEdit = {orderitemdetailswrapped: OrderItemDetailsWrapped list; suborderwrapped: SubOrderWrapped list; currentsuborderid: int}

type invoiceModel = {daticliente: string; orderitemdetailswrapped: OrderItemDetailsWrapped list; nextavailablenumber: int; idname: indexNameDataRecord list}
let printWholeOrderInvoiceAsWordformat orderId =
    log.Debug(sprintf "%s %d " "printWholeOrderInvoiceAsWordformat" orderId)
    let ctx = Db.getContext()
    let orderItemDetails = Db.getOrderItemDetailOfOrderThatArenotInInitState orderId ctx
    let order = Db.Orders.getOrder orderId ctx

    let allCustomers = Db.getAllCustomers ctx
    let idNameCustomers = allCustomers |> List.map (fun (x:Db.CustomerData) -> {index=x.Customerdataid;name=x.Name;data=x.Data})

    let orderItemDetailsWrappedList = orderItemDetails |> List.map (fun (x:Db.OrderItemDetails) -> DbObjectWrapper.WrapOrderItemDetails(x) "") //  DbWrappedEntities.DbObjectWrapper.WrapOrderItemDetails(orderItemDetails)

    let orderItemDetailsArray = orderItemDetails |> List.toArray

    choose [
        GET >=> warbler (fun _ ->
            let datiCliente = {daticliente = "pinco pallo"; orderitemdetailswrapped = orderItemDetailsWrappedList; nextavailablenumber = 1; idname = idNameCustomers  }
            DotLiquid.page("invoiceData.html") datiCliente
        )
        POST >=> bindToForm Form.invoiceForm (
            fun form -> 
                let fileName = sprintf "fattura_%d.docx" System.DateTime.Now.Ticks
                let border = new Border()
                let secondBorder = new Border()
                let _ = secondBorder.Color = Color.Black

                let _ = border.Color <- Color.Black
                let doc = DocX.Create(fileName)
                let _ = doc.InsertParagraph(form.Comment)
                let _ = doc.InsertParagraph(" ")
                let _ = doc.InsertParagraph(" ")
                let _ = doc.InsertParagraph(" ")

                let headerTable = doc.InsertTable(1,3)

                let _ = headerTable.SetBorder(TableBorderType.Top,secondBorder)
                let _ = headerTable.SetBorder(TableBorderType.Bottom,secondBorder)
                let _ = headerTable.SetBorder(TableBorderType.Right,secondBorder)
                let _ = headerTable.SetBorder(TableBorderType.Left,secondBorder)

                let _ = headerTable.Rows.[0].Cells.[0].InsertParagraph("Qta")
                let _ = headerTable.Rows.[0].Cells.[1].InsertParagraph("Item")
                let _ = headerTable.Rows.[0].Cells.[2].InsertParagraph("Prezzo")

                let itemTable = doc.InsertTable(List.length orderItemDetails,3)
                let _ = itemTable.SetBorder(TableBorderType.Top,border)
                let _ = itemTable.SetBorder(TableBorderType.Bottom,border)
                let _ = itemTable.SetBorder(TableBorderType.Right,border)
                let _ = itemTable.SetBorder(TableBorderType.Left,border)

                let _ = [0 .. ((List.length orderItemDetails)-1)] |> List.iter (fun i -> 
                    (itemTable.Rows.[i].Cells.[0].InsertParagraph(orderItemDetailsArray.[i].Quantity |> string) |> ignore;  
                    (itemTable.Rows.[i].Cells.[1].InsertParagraph(orderItemDetailsArray.[i].Name) |> ignore;  ))

                    (itemTable.Rows.[i].Cells.[2].InsertParagraph(sprintf "%.2f" (Utils.unbundleVat  (orderItemDetailsArray.[i].Price) Globals.ALIQUOTA_IVA_UNICA)  ) |> ignore;))  

                let _ = match (order.Total - order.Adjustedtotal) with
                    | 0M  -> 
                        let itemTablePrice = doc.InsertTable(2,3)
                        let _ = itemTablePrice.Rows.[0].Cells.[2].InsertParagraph("_________")
                        let _ = itemTablePrice.Rows.[1].Cells.[1].InsertParagraph("totale:")
                        let _ = itemTablePrice.Rows.[1].Cells.[2].InsertParagraph(sprintf "%.2f" order.Adjustedtotal)
                        ()

                    | _ ->  
                        let itemTablePrice = doc.InsertTable(2,3)
                        let _ = itemTablePrice.Rows.[0].Cells.[2].InsertParagraph("_________")

                        let _ = itemTablePrice.Rows.[1].Cells.[1].InsertParagraph("totale lordo:")
                        let _ = itemTablePrice.Rows.[1].Cells.[2].InsertParagraph(sprintf "%.2f" order.Total)

                        let itemTablePrice = doc.InsertTable(1,3)
                        let _ = itemTablePrice.Rows.[0].Cells.[1].InsertParagraph("sconto:")
                        let _ = itemTablePrice.Rows.[0].Cells.[2].InsertParagraph(sprintf "%.2f" (order.Total - order.Adjustedtotal))

                        let itemTablePrice = doc.InsertTable(2,3)
                        let _ = itemTablePrice.Rows.[0].Cells.[2].InsertParagraph("_________")
                        let _ = itemTablePrice.Rows.[1].Cells.[1].InsertParagraph("totale netto:")
                        let _ = itemTablePrice.Rows.[1].Cells.[2].InsertParagraph(sprintf "%.2f" order.Adjustedtotal)
                        ()

                let _ = doc.Save()
                Redirection.found Path.Orders.seeDoneOrders
        )

    ]


let printSubOrderInvoice subOrderId =
    log.Debug(sprintf "%s %d " "printSubOrderInvoice" subOrderId)
    let ctx = Db.getContext()

    let orderItemsDetails = Db.getOrderItemDetailsOfSubOrderThatAreNotInInitState subOrderId ctx

    let allCustomers = Db.getAllCustomers ctx
    let idNameCustomers = allCustomers |> List.map (fun (x:Db.CustomerData) -> {index=x.Customerdataid;name=x.Name;data=x.Data})

    let nextAvailableNumber = 
        match Db.latestInvoiceNumber ctx with
        | Some N -> N + 1
        | None -> 1

    let orderItemDetailsWrappedList = orderItemsDetails |> List.map (fun (x:Db.OrderItemDetails) -> DbObjectWrapper.WrapOrderItemDetails(x) "") 

    let totalOfSuborder = orderItemsDetails |> List.map (fun (x:Db.OrderItemDetails) -> x.Price) |> List.fold (+) 0M

    choose [
        GET >=> warbler (fun _ ->
            let datiCliente = {daticliente = ""; orderitemdetailswrapped = orderItemDetailsWrappedList; nextavailablenumber = nextAvailableNumber; idname=idNameCustomers }
            DotLiquid.page("invoiceData.html") datiCliente
        )

        POST >=> bindToForm Form.invoiceForm (
            fun form -> 

                let printerForReceipts = Db.getPrintersForReceipts ctx
                let printerNames = printerForReceipts |> List.map (fun (x:Db.Printer) -> x.Name)

                let customerDataId = 
                    match (form.CompanyId,form.StoreCompany) with 
                    | (-1M, Some _) -> 
                        let newCustomer = Db.createCustmomerData form.CompanyName form.Comment ctx
                        newCustomer.Customerdataid
                    | (X,_)  when (X <> -1M) ->
                        Db.tryUpdateCustomerData ((int)X) form.CompanyName form.Comment ctx
                        (int)X
                    | _ -> -1


                let _ = removeSpooledFiles()

                let now = System.DateTime.Now.ToLocalTime()

                let unBoundledTotal = Utils.unbundleVat totalOfSuborder Globals.ALIQUOTA_IVA_UNICA

                let showDetails = 
                    match form.ShowDetails with 
                    | Some _ -> (orderItemsDetails |> (List.fold (fun y (x:Db.OrderItemDetails) ->  y + (sprintf "%d %-20s %-10.2f\n"    x.Quantity  x.Name  x.Price )) ""))  
                    | None -> ""

                let text = "fattura: n."+(form.InvoiceNumber|> string)+"\n\n"+ "data:"+now.ToString() + "\n\n" + form.Comment + "\n\n" +
                    showDetails + "\n"+(sprintf "%-22s %-40.2f\n" "totale: " totalOfSuborder)+ 
                    "\n\n" + (sprintf "imponibile: %-30.2f"  unBoundledTotal)+"\n\nIVA " + (sprintf "%.0f %%: " Globals.ALIQUOTA_IVA_UNICA) + (sprintf "%.2f" (totalOfSuborder - unBoundledTotal))+ "\n\n"

                let invoice = match customerDataId with
                      | -1 -> Db.createInvoiceBySubOrderIdWithNoCustomerId subOrderId text ((int)form.InvoiceNumber) ctx
                      | _ ->  Db.createInvoiceBysubOrderIdAndCustomerId subOrderId customerDataId text ((int)form.InvoiceNumber) ctx                

                let fileName = sprintf "receipt_print%d.txt" System.DateTime.Now.Ticks
                let outFile = new System.IO.StreamWriter(fileName,true,Encoding.UTF8)
                let _ = outFile.WriteLine(text)
                let _ = outFile.Close()
                let _ = printerNames |> List.iter (fun x -> 
                    System.Diagnostics.Process.Start(Settings.Printcommand, "-P" + x + " " + AppDomain.CurrentDomain.BaseDirectory + fileName) |> ignore
                    File.Copy(fileName,x+fileName.Replace(".txt",".ok"),true)
                )

                Db.setSubOrderAsPayed subOrderId ctx

                let orderId = Db.getOrderIdOfSubOrder subOrderId ctx

                Redirection.found (sprintf Path.Orders.subdivideDoneOrder orderId)
        )
    ]

let printWholeOrderInvoice orderId =
    log.Debug(sprintf "%s %d" "printWholeOrderInvoice" orderId)
    let ctx = Db.getContext()
    let orderItemsDetails = Db.getOrderItemDetailOfOrderThatArenotInInitState orderId ctx
    let order = Db.Orders.getOrder orderId ctx

    let allCustomers = Db.getAllCustomers ctx
    let idNameCustomers = allCustomers |> List.map (fun (x:Db.CustomerData) -> {index=x.Customerdataid;name=x.Name; data=x.Data})

    let nextAvailableNumber = 
        match Db.latestInvoiceNumber ctx with
        | Some N -> N + 1
        | None -> 1

    let orderItemDetailsWrappedList = orderItemsDetails |> List.map (fun (x:Db.OrderItemDetails) -> DbObjectWrapper.WrapOrderItemDetails(x) "") 


    choose [
        GET >=> warbler (fun _ ->
            let datiCliente = {daticliente = ""; orderitemdetailswrapped = orderItemDetailsWrappedList; nextavailablenumber = nextAvailableNumber; idname  = idNameCustomers }
            DotLiquid.page("invoiceData.html") datiCliente
        )

        POST >=> bindToForm Form.invoiceForm (
            fun form -> 
                let printerForReceipts = Db.getPrintersForReceipts ctx
                let printerNames = printerForReceipts |> List.map (fun (x:Db.Printer) -> x.Name)

                let customerDataId = 
                    match (form.CompanyId,form.StoreCompany) with 
                    | (-1M, Some _) -> 
                        let newCustomer = Db.createCustmomerData form.CompanyName form.Comment ctx
                        newCustomer.Customerdataid
                    | (X,_)  when (X <> -1M) ->
                        Db.tryUpdateCustomerData ((int)X) form.CompanyName form.Comment ctx
                        (int)X
                    | _ -> -1

                let _ = removeSpooledFiles()

                let textAboutTotal =   
                    match (order.Adjustispercentage,order.Adjustisplain) with
                    | (true,false) -> sprintf "sconto percentuale: %.2f%%\n%s %.2f"  order.Percentagevariataion "Totale scontato" order.Adjustedtotal
                    | (false,true) -> sprintf "sconto: %.2f\n%s %.2f"  order.Plaintotalvariation  "Totale scontato" order.Adjustedtotal
                    | _ -> ""

                let now = System.DateTime.Now.ToLocalTime()

                let unBoundledTotal = Utils.unbundleVat order.Adjustedtotal Globals.ALIQUOTA_IVA_UNICA

                let showDetails = 
                    match form.ShowDetails with 
                    | Some _ -> (orderItemsDetails |> (List.fold (fun y (x:Db.OrderItemDetails) ->  y + (sprintf "%d %-20s %-10.2f\n"    x.Quantity  x.Name  x.Price )) ""))  
                    | None -> ""

                let text = "fattura: n. "+(form.InvoiceNumber|> string)+"\n\n"+ "data:"+now.ToString() + "\n\n" + form.Comment + "\n\n"+
                    showDetails + "\nTotale: "+(order.Total |> string)+ "\n"+textAboutTotal +
                    "\n\n" + "imponibile: " + (sprintf "%.2f" unBoundledTotal)+"\n\nIVA " + (sprintf "%.0f %%: " Globals.ALIQUOTA_IVA_UNICA) + (sprintf "%.2f" (order.Adjustedtotal - unBoundledTotal))+ "\n\n"

                let invoice = match customerDataId with
                              | -1 -> Db.createInvoiceByOrderIdWithNoCustomerId orderId text ((int)form.InvoiceNumber) ctx
                              | _ ->  Db.createInvoiceByOrderIdAndCustomerId orderId customerDataId text ((int)form.InvoiceNumber) ctx

                let fileName = sprintf "receipt_print%d.txt" System.DateTime.Now.Ticks
                let outFile = new System.IO.StreamWriter(fileName,true,Encoding.UTF8)
                let _ = outFile.WriteLine(text)
                let _ = outFile.Close()
                let _ = printerNames |> List.iter (fun x -> 
                    System.Diagnostics.Process.Start(Settings.Printcommand, "-P" + x + " " + AppDomain.CurrentDomain.BaseDirectory + fileName) |> ignore
                    File.Copy(fileName,x+fileName.Replace(".txt",".ok"),true)
                )
                let _ = archiveOrderByUserId orderId

                Redirection.found Path.Orders.seeDoneOrders
        )
    ]



    
let printWholeOrderReceipt orderId =
    log.Debug(sprintf "%s %d " "printWholeOrderReceipt" orderId)
    let ctx = Db.getContext()
    let order = Db.Orders.getOrder orderId ctx
    let orderItemsDetails = Db.getOrderItemDetailOfOrderThatArenotInInitState orderId ctx
    let printerForReceipts = Db.getPrintersForReceipts ctx
    let printerNames = printerForReceipts |> List.map (fun (x:Db.Printer) -> x.Name)

    let _ = removeSpooledFiles()

    let text = Utils.textForWholeOrderReceipt orderId orderItemsDetails ctx

    let fileName = sprintf "receipt_print%d.txt" System.DateTime.Now.Ticks
    let outFile = new System.IO.StreamWriter(fileName,true,Encoding.UTF8)
    let _ = outFile.WriteLine(text)
    let _ = outFile.Close()
    let _ = printerNames |> List.iter (fun x -> 
        System.Diagnostics.Process.Start(Settings.Printcommand, "-P" + x + " " + AppDomain.CurrentDomain.BaseDirectory + fileName) |> ignore
        File.Copy(fileName,x+fileName.Replace(".txt",".ok"),true)
    )

    let _ = archiveOrderByUserId orderId 

    Redirection.found (Path.Orders.seeDoneOrders)


type PaymentItemsLiquidValues = {wrappedSubOrder: DbWrappedEntities.SubOrderWrapped; wrappedOrderItems: DbWrappedEntities.OrderItemDetailsWrapped list;   table: string;  wrappedPaymentItems: DbWrappedEntities.PaymentItemWrapped list;tenderCodes: indexNameRecord list;residualPaymentDue: decimal;orderId: int;subOrderId:int}


let subOrderPaymentItems subOrderId orderId (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d" "subOrderPaymentItems" subOrderId )
    let ctx = Db.getContext()
    warbler (fun x -> 

        let _ = match (x.request.queryParam("tenderid"),x.request.queryParam("amount")) with 
             | (Choice1Of2 tenderid,Choice1Of2 amount) -> Db.addPaymentItemToSubOrder subOrderId ((int)tenderid) ((decimal)amount) ctx
             | _ -> ()

        let _ = match (x.request.queryParam("adjustment")) with
             | (Choice1Of2 adjustment) -> 
                Db.Orders.setAdjustmentOfSubOrder ((int)subOrderId) ((decimal)adjustment) ctx
                Db.Orders.setPercentAdjustmentOfSubOrder ((int)subOrderId) 0M ctx
             | _ -> ()

        let _ = match (x.request.queryParam("percentadjustment")) with
             | (Choice1Of2 percentadjustment) ->
                 Db.Orders.setPercentAdjustmentOfSubOrder ((int)subOrderId) ((decimal)percentadjustment) ctx
                 Db.Orders.setAdjustmentOfSubOrder ((int)subOrderId) 0M ctx;
             | _ -> ()


        let table = Db.Orders.getTableOfOrder orderId ctx
        let subOrder = Db.Orders.getSubOrder subOrderId ctx

        let subOrderItemDetails = Db.Orders.getOrderItemDetailsOfSubOrder subOrderId ctx
        let wrappedOrderItemDetails = subOrderItemDetails |>  Seq.map (fun x -> DbWrappedEntities.DbObjectWrapper.WrapOrderItemDetails(x) "") |> Seq.toList

        let tenderCodes = Db.getAllTenderCodes ctx
        let tenderCodesIndexNameList = tenderCodes |> List.map (fun (x:Db.TenderCode) -> {index=x.Tendercodesid; name=x.Tendername})
        let paymentItems = Db.Orders.getPaymentItemsOfSubOrder subOrderId ctx
        let paymentItemDetails = Db.Orders.getPaymentItemDetailsOfSubOrder subOrderId ctx
        let adjustment = if (subOrder.Subtotaladjustment <> 0M) then subOrder.Subtotaladjustment else Math.Round ((subOrder.Subtotal*(subOrder.Subtotalpercentadjustment/100M)),2,MidpointRounding.AwayFromZero)

        let residual = subOrder.Subtotal + adjustment - ((paymentItemDetails |> List.map (fun (x:Db.PaymentItemDetail) -> x.Amount ) |> List.fold (+) 0.0M))

        let wrappedPaymentItems = paymentItemDetails |> List.map (fun (x:Db.PaymentItemDetail) -> DbWrappedEntities.DbObjectWrapper.WrapPaymentItem(x))
        let wrappedSubOrder = DbWrappedEntities.DbObjectWrapper.WrapSubOrder(subOrder) ""
        let liquidModel = {wrappedSubOrder = wrappedSubOrder; wrappedOrderItems= wrappedOrderItemDetails; table=table; wrappedPaymentItems = wrappedPaymentItems; tenderCodes = tenderCodesIndexNameList;residualPaymentDue=residual;orderId = orderId;subOrderId=subOrderId }
        DotLiquid.page("subOrderPaymentItem.html") liquidModel
    )

let removeAllDiscountOfSubOrder subOrderId =
    let ctx = Db.getContext()
    let subOrder = Db.Orders.getSubOrder subOrderId ctx
    let _ = subOrder.Subtotaladjustment <- 0M
    let _ = subOrder.Subtotalpercentadjustment <-0M
    ctx.SubmitUpdates()
    Redirection.found (sprintf Path.Orders.subOrderPaymentItems subOrderId subOrder.Orderid)




type PaymentItemsLiquidValuesForOrder = {wrappedOrder: DbWrappedEntities.OrderWrapped; wrappedOrderItems: DbWrappedEntities.OrderItemDetailsWrapped list; table: string; wrappedPaymentItems: DbWrappedEntities.PaymentItemWrapped list;tenderCodes: indexNameRecord list;residualPaymentDue: decimal;orderId: int}
let wholeOrderPaymentItems  orderId  =
    log.Debug(sprintf "%s %d " "wholeOrderPaymentItems" orderId )
    let ctx = Db.getContext()
    warbler (fun x -> 

        let _ = match (x.request.queryParam("tenderid"),x.request.queryParam("amount")) with 
             | (Choice1Of2 tenderid,Choice1Of2 amount) -> Db.addPaymentItemToOrder orderId ((int)tenderid) ((decimal)amount) ctx
             | _ -> ()

        let orderItemsDetails = Db.getOrderItemDetailsOfOrder orderId ctx
        let wrappedOrderItemsDetails = orderItemsDetails |> Seq.map (fun x -> DbWrappedEntities.DbObjectWrapper.WrapOrderItemDetails(x) "") |> Seq.toList;

        let order = Db.Orders.getOrder orderId ctx
        let tenderCodes = Db.getAllTenderCodes ctx
        let tenderCodesIndexNameList = tenderCodes |> List.map (fun (x:Db.TenderCode) -> {index=x.Tendercodesid; name=x.Tendername})
        let paymentItems = Db.Orders.getPaymentItemsOfOrder orderId ctx
        let paymentItemDetails = Db.Orders.getPaymentItemDetailsOfOrder orderId ctx
        let residual = order.Adjustedtotal - ((paymentItemDetails |> List.map (fun (x:Db.PaymentItemDetail) -> x.Amount ) |> List.fold (+) 0.0M))

        let wrappedPaymentItems = paymentItemDetails |> List.map (fun (x:Db.PaymentItemDetail) -> DbWrappedEntities.DbObjectWrapper.WrapPaymentItem(x))
        let wrappedOrder = DbWrappedEntities.DbObjectWrapper.WrapOrder(order) 
        let liquidModel = {wrappedOrder = wrappedOrder; table = order.Table ; wrappedOrderItems = wrappedOrderItemsDetails; wrappedPaymentItems = wrappedPaymentItems; tenderCodes = tenderCodesIndexNameList;residualPaymentDue=residual;orderId = orderId}
        DotLiquid.page("wholeOrderPaymentItem.html") liquidModel
    )





let removePaymentItemOfSubOrder paymentItemId subOrderId orderId  (user:UserLoggedOnSession) =
    let ctx = Db.getContext()
    Db.Orders.removePaymentItem paymentItemId ctx

    Redirection.found (sprintf Path.Orders.subOrderPaymentItems subOrderId orderId)

let removePaymentItemOfOrder paymentItemId  orderId   =
    let ctx = Db.getContext()
    Db.Orders.removePaymentItem paymentItemId ctx

    Redirection.found (sprintf Path.Orders.wholeOrderPaymentItems  orderId)


let setSubOrderAsPaid subOrderId orderId (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d " "setSubOrderAsPaid" subOrderId)
    let ctx = Db.getContext()
    let subOrder = Db.Orders.getSubOrder subOrderId ctx
    let _ = subOrder.Payed <- true
    ctx.SubmitUpdates()
    Redirection.found (sprintf Path.Orders.subdivideDoneOrder orderId)

let printReceipt subOrderId orderId =
    log.Debug(sprintf "%s %d %d" "printReceiptXXX" subOrderId orderId )
    let ctx = Db.getContext()
    let subOrder = Db.Orders.getSubOrder subOrderId ctx
    let printerForReceipts = Db.getPrintersForReceipts ctx
    let printerNames = printerForReceipts |> List.map (fun (x:Db.Printer) -> x.Name)

    let orderItemOfSubOrder = Db.getOrderItemDetailsOfSubOrderThatAreNotInInitState subOrder.Suborderid ctx
    let total = orderItemOfSubOrder |> List.fold (fun accumul (x:Db.OrderItemDetails) -> accumul + (x.Price)) ((decimal)0.0)

    let text = Utils.textForSubOrderReceipt orderItemOfSubOrder ctx

    let fileName = sprintf "receipt_print%d.txt" System.DateTime.Now.Ticks
    let outFile = new System.IO.StreamWriter(fileName,true,Encoding.UTF8)
    let _ = outFile.WriteLine(text)
    let _ = outFile.Close() |> ignore
    let _ = printerNames |> List.iter (fun x -> 
        System.Diagnostics.Process.Start(Settings.Printcommand, "-P" + x + " " + AppDomain.CurrentDomain.BaseDirectory + fileName) |> ignore
        File.Copy(fileName,x+fileName.Replace(".txt",".ok"),true)
    )
    subOrder.Payed <- true
    ctx.SubmitUpdates()

    Redirection.found (sprintf Path.Orders.subdivideDoneOrder orderId)



let setSubOrderAsNotPaid subOrderId orderId (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d %d"  "setSubOrderAsNotPaid" subOrderId orderId)
    let ctx = Db.getContext()
    let subOrder = Db.Orders.getSubOrder subOrderId ctx
    let _ = subOrder.Payed <- false
    ctx.SubmitUpdates()
    Redirection.found (sprintf Path.Orders.subdivideDoneOrder orderId)


let deleteSubOrder subOrderId orderId (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d %d" "deleteSubOrder" subOrderId orderId)
    let ctx = Db.getContext()
    Db.safeDeleteSubOrder subOrderId ctx
    Redirection.found (sprintf Path.Orders.subdivideDoneOrder orderId)

type LiquidWrappedOrderItems = {orderitemdetailswrapped: OrderItemDetailsWrapped list; 
    suborderwrapped: SubOrderWrapped list; orderid: int; table: string; encodedbackurl: string; tendercodes: TenderCodeWrapped list }

 type LiquidWrapperOrderItemsWithGenericCourses = { orderitemdetailswrapped: OrderItemDetailsWrapped list; 
     suborderwrapped: SubOrderWrapped list; orderid: int; coursewrappedlist: CourseWrapped list; encodedbackurl: string; }

let recomputeSubOrderTotal (subOrder:Db.SubOrder) =
    let ctx = Db.getContext()
    let orderItemsSOfSuborder = Db.Orders.getOrderItemsOfSubOrder subOrder.Suborderid ctx
    let total = orderItemsSOfSuborder |> List.map (fun (x:Db.OrderItem) -> x.Price) |> List.sum
    subOrder.Subtotal <- total
    ctx.SubmitUpdates()


let colapseDoneOrder id (user: UserLoggedOnSession) =
    log.Debug(printf "%s %d" "colapseDoneOrder" id)
    let ctx = Db.getContext()

    choose [
        GET >=> warbler ( fun (x:HttpContext) ->

            let abstractCourses = Db.Courses.getAllAbstractCourses ctx
            let abstractCoursesWrapped = abstractCourses |> List.map (fun x -> DbObjectWrapper.WrapCourse x)

            let idsOfNonUnitaryOrderItems = Db.getIdsOfNonUnitaryOrderItemsOfOrder id ctx
            let _ = idsOfNonUnitaryOrderItems |> Seq.iter (fun x -> Db.splitOrderItemInToUnitaryOrderItems x ctx)

            let order = Db.getOrderDetail id ctx
            let orderItemsdetailsOfOrder = Db.getOrderItemDetailOfOrderDetailThatAreNotInInitState order ctx

            let subOrders = 
                Db.Orders.getSubOrdersOfOrderById id ctx

            let _ = subOrders |> List.iter (fun (x:Db.SubOrder) -> recomputeSubOrderTotal x )

            let colors = Globals.getFirstNColorValues (List.length subOrders)

            let wrappedSubOrders =  List.map2 (fun (x:Db.SubOrder) y -> DbObjectWrapper.WrapSubOrder x y) subOrders colors

            let subOdersColorsMap = wrappedSubOrders |> List.map (fun (x:SubOrderWrapped) -> (x.Suborderid,x.Csscolor)) |> Map.ofList

            let wrappedOrderItemDetails = 
                List.map (fun (x:Db.OrderItemDetails) -> DbObjectWrapper.WrapOrderItemDetails x (if (x.Isinsasuborder) then subOdersColorsMap.[x.Suborderid] else "#dee7ed")) orderItemsdetailsOfOrder 

            let parametersNames = wrappedOrderItemDetails |> 
                List.map (fun (x:OrderItemDetailsWrapped) -> "orderitem"+(x.Orderitemid|> string))

            let parametersFromRequest = parametersNames |> 
                List.filter (fun z -> match (x.request.queryParam(z)) with | Choice1Of2 _ -> true | _ -> false) |>
                    List.map (fun z -> z.Substring("orderitem".Length)) |> List.map (fun z -> (int) z)

            let abstractCurseId = match x.request.queryParam("piatto") with | Choice1Of2 x -> x | _ -> "99"
            log.Debug(sprintf "%s %s" "piatto" abstractCurseId)

            if (parametersFromRequest.Length>0) then (
                let subGroup = Db.unBoundDifferentSubGroupsOfOrderItemsByIs parametersFromRequest ctx

                parametersFromRequest |> List.iter (fun z -> Db.safeRemoveOrderItem z ctx)
                Db.createPlainUnitaryOrderItemById id (abstractCurseId|>int) subGroup ctx

                Redirection.found (sprintf Path.Orders.colapseDoneOrder id)
            ) else 

                let liquidItem = {orderitemdetailswrapped = wrappedOrderItemDetails; suborderwrapped = wrappedSubOrders; orderid=id; encodedbackurl = WebUtility.UrlEncode (sprintf Path.Orders.subdivideDoneOrder id); coursewrappedlist= abstractCoursesWrapped }
                DotLiquid.page("colapseDoneOrder.html") liquidItem
        )
    ]

let subdivideDoneOrder id (user: UserLoggedOnSession)  = 
    log.Debug(sprintf "%s %d" "subdivideDoneOrder" id)
    let ctx = Db.getContext()
    choose [
        GET >=> warbler ( fun (x:HttpContext) ->

            let idsOfNonUnitaryOrderItems = Db.getIdsOfNonUnitaryOrderItemsOfOrder id ctx
            let _ = idsOfNonUnitaryOrderItems |> Seq.iter (fun x -> Db.splitOrderItemInToUnitaryOrderItems x ctx)
            let order = Db.getOrderDetail id ctx
            let orderItemsdetailsOfOrder = Db.getOrderItemDetailOfOrderDetailThatAreNotInInitState order ctx
            let dbTenderCodes = Db.getAllTenderCodes ctx
            let wrappedTenderCodes = dbTenderCodes |> List.map (fun (x:Db.TenderCode) -> DbObjectWrapper.WrapTenderCode x)
            let subOrders = 
                Db.Orders.getSubOrdersOfOrderById id ctx

            let _ = subOrders |> List.iter (fun (x:Db.SubOrder) -> recomputeSubOrderTotal x )
            let colors = Globals.getFirstNColorValues (List.length subOrders)
            let wrappedSubOrders =  List.map2 (fun (x:Db.SubOrder) y -> DbObjectWrapper.WrapSubOrder x y) subOrders colors
            let subOdersColorsMap = wrappedSubOrders |> List.map (fun (x:SubOrderWrapped) -> (x.Suborderid,x.Csscolor)) |> Map.ofList
            let wrappedOrderItemDetails = 
                List.map (fun (x:Db.OrderItemDetails) -> DbObjectWrapper.WrapOrderItemDetails x (if (x.Isinsasuborder) then subOdersColorsMap.[x.Suborderid] else "#dee7ed")) orderItemsdetailsOfOrder 
            let parametersNames = wrappedOrderItemDetails |> 
                List.map (fun (x:OrderItemDetailsWrapped) -> "orderitem"+(x.Orderitemid|> string))
            let parametersFromRequest = parametersNames |> 
                List.filter (fun z -> match (x.request.queryParam(z)) with | Choice1Of2 _ -> true | _ -> false) |>
                    List.map (fun z -> z.Substring("orderitem".Length)) |> List.map (fun z -> (int) z)
            if (parametersFromRequest.Length>0) then (
                let subOrder = Db.Orders.createSubOrderOfOrderItem id ctx
                parametersFromRequest |> List.iter (fun z -> Db.bindOrderItemToSubOrder z subOrder.Suborderid ctx)
                Redirection.found (sprintf Path.Orders.subdivideDoneOrder id)
            ) else 

                let liquidItem = {orderitemdetailswrapped = wrappedOrderItemDetails; suborderwrapped = wrappedSubOrders; orderid=id; table=order.Table; encodedbackurl = WebUtility.UrlEncode (sprintf Path.Orders.subdivideDoneOrder id);tendercodes =  wrappedTenderCodes}
                DotLiquid.page("subdivideDoneOrder.html") liquidItem
        )
    ]

let splitOrderItemInToUnitaryOrderItems id (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d " "splitOrderItemInToUnitaryOrderItems" id)
    let ctx = Db.getContext()
    let theOrderItem = Db.getTheOrderItemById id ctx
    let connectedOrderItemStates = theOrderItem.``public.orderitemstates by orderitemid`` |> Seq.toList
    let ingredientdecrements = theOrderItem.``public.ingredientdecrement by orderitemid`` |> Seq.toList
    let rejectOrderItems = theOrderItem.``public.rejectedorderitems by orderitemid`` |> Seq.toList
    let variations = theOrderItem.``public.variations by orderitemid`` |> Seq.toList
    let courseId = theOrderItem.Courseid
    let orderId = theOrderItem.Orderid
    let comment = theOrderItem.Comment
    let outGroupId = theOrderItem.Ordergroupid

    let clonedOrderItems = [1 .. theOrderItem.Quantity] |> List.map (fun _ -> Db.createUnitaryNakedOrderItemByOrderId courseId orderId comment theOrderItem.Price outGroupId ctx) // |> List.fold (@) []
    
    let clonedIngredientDecrements = clonedOrderItems  |> List.map (fun (x:Db.OrderItem) -> ingredientdecrements |> List.map (fun (y:Db.IngredientDecrement) -> Db.createClonedIngredientDecrement x.Orderitemid ((decimal) theOrderItem.Quantity)  y ctx)) |> List.fold (@) []
    let clonedVariations = clonedOrderItems |> List.map (fun (x:Db.OrderItem) -> variations |> List.map  (fun (y:Db.Variation) -> Db.createClonedVariationOfOrderItem x.Orderitemid y.Ingredientid y.Tipovariazione ctx)) |> List.fold (@) []
    let clonedRejectedOrderItems = clonedOrderItems |> List.map (fun (x:Db.OrderItem) -> rejectOrderItems |> List.map (fun (y:Db.RejectedOrderItems) -> Db.createClonedRejectedOrderItem x.Orderitemid y ctx )) |> List.fold (@) []
    let clonedOrderItemStates = clonedOrderItems |> List.map (fun (x:Db.OrderItem) -> connectedOrderItemStates |> List.map (fun (y:Db.OrderItemState) -> Db.createClonedOrderItemState x.Orderitemid y ctx)) |> List.fold (@) []
    
    let _ = Db.safeRemoveOrderItem theOrderItem.Orderitemid ctx
    Redirection.FOUND (sprintf Path.Orders.subdivideDoneOrder theOrderItem.Orderid)
            
let editDoneOrder id (user:UserLoggedOnSession) =

    let thisUrl = sprintf Path.Orders.editDoneOrder id
    log.Debug(sprintf "%s %d" "editDoneOrder" id )

    let ctx = Db.getContext()
    let dbUser = Db.getUserById user.UserId ctx
    match (dbUser.Canmanageallorders || user.Role = "admin") with
    | true ->

     choose [
        GET >=> warbler (fun _ ->
            let order = Db.Orders.getOrder id ctx
            let orderItemsdetailsOfOrder = Db.getOrderItemDetailOfOrder order ctx
            html (View.seeOrder order orderItemsdetailsOfOrder thisUrl) 
         )

        POST >=> bindToForm Form.priceAdjustment (fun form -> 
            let order = Db.Orders.getOrder id ctx

            let setPercentageOrPlainPriceVariation =
                match (form.PercentOrValue,form.Value) with 
                | (_,X)  when (Decimal.Parse(X)  = (decimal)0) ->
                
                                order.Adjustispercentage <- false
                                order.Adjustisplain <- false
                                order.Percentagevariataion <- (decimal)0
                                order.Plaintotalvariation <- (decimal)0
                                order.Adjustedtotal <- order.Total
                | ("PERCENTUALE",_) -> 
                                   order.Adjustispercentage <- true
                                   order.Adjustisplain<- false 
                                   order.Plaintotalvariation <- (decimal) 0 
                                   order.Percentagevariataion <- Decimal.Parse(form.Value)
                                   order.Adjustedtotal <- order.Total + order.Total * order.Percentagevariataion/(decimal)100
                | ("VALORE",_) -> order.Adjustispercentage <- false
                                  order.Adjustisplain <- true
                                  order.Plaintotalvariation <-  Decimal.Parse(form.Value)
                                  order.Percentagevariataion <- (decimal)0
                                  order.Adjustedtotal <- order.Total + (Decimal.Parse(form.Value))
                | _ -> order.Adjustispercentage <- false
                       order.Adjustisplain <- false
                               
            ctx.SubmitUpdates()
            let redirTo = Path.Orders.editDoneOrder.Value
            let redirToTrimmed = redirTo.Substring(0,redirTo.IndexOf("%"))
            Redirection.FOUND (redirToTrimmed+(id |> string))
        )
     ]
    | _ -> Redirection.FOUND Path.home

let optimizeVoided = warbler (fun _ ->
    let ctx = Db.getContext()
    let voidedOrders = Db.Orders.getVoidedOrders ctx
    let _ = List.iter (fun (x:Db.Order) -> Db.safeRemoveOrder x.Orderid ctx ) voidedOrders
    ctx.SubmitUpdates()
    Redirection.FOUND Path.home
)

        
let deArchiveLatestOrder (user:UserLoggedOnSession) =
    log.Debug("deArchiveLatestOrder")
    let ctx = Db.getContext()
    let dbUser = Db.getUserById user.UserId ctx
    let _ = 
        if (user.Role = "admin " || dbUser.Canmanageallorders) then
             (let latestLogOrder = Db.getLatestLogOrder ctx
              match latestLogOrder with
              | Some theLatestOrder ->
                log.Debug("got an order");
                let order = Db.Orders.getOrder theLatestOrder.Orderid  ctx
                order.Archived <- false
                theLatestOrder.Delete()
                ctx.SubmitUpdates()
              | _ -> 
                log.Debug("got nothing")
                ()
             )
    Redirection.found Path.Orders.seeDoneOrders

let switchCourseCategoryVisibility categoryId  =
    log.Debug(sprintf "%s %d\n" "switchCourseCategoryVisibility" categoryId )
    let ctx = Db.getContext()
    let courseCategory = Db.Courses.tryGetCourseCategory categoryId ctx
    let _ = match courseCategory with
        | Some X -> X.Visibile <- not X.Visibile; ctx.SubmitUpdates()
        | None -> ()
    Redirection.found Path.Courses.adminCategories

let rejectOrderItem orderItemId (user:UserLoggedOnSession) =
    log.Debug(sprintf "%s %d \n" "rejectOrderItem" orderItemId )
    let ctx = Db.getContext()
    let theOrderItemDetail = Db.Orders.getOrderItemDetail orderItemId ctx
    let roleId = user.RoleId
    let stateId = theOrderItemDetail.Stateid
    let categoryId = theOrderItemDetail.Categoryid

    match Db.isEnablerRoleCatState roleId categoryId stateId ctx with
    | true ->
      choose [
        GET >=> warbler (fun x ->
            // x.request.
            View.rejectOrderItem theOrderItemDetail |> html
        )
        POST >=> bindToForm Form.orderItemRejection (fun form ->
            let _ = Db.createRejectedOrderItem theOrderItemDetail.Orderitemid theOrderItemDetail.Courseid form.Motivation ctx
            let _ = Db.setOrderItemAsRejected theOrderItemDetail.Orderitemid ctx
            let _ = Db.reinitializeOrderItemState theOrderItemDetail.Orderitemid ctx
            Redirection.FOUND Path.Orders.orderItemsProgress
        )
      ]
    | false -> Redirection.FOUND Path.Orders.orderItemsProgress

let resetDiscount orderId = 
    log.Debug(sprintf "%s %d\n" "resetDiscount" orderId)
    let ctx = Db.getContext()
    let order = Db.Orders.getOrder orderId ctx
    order.Adjustispercentage <- false
    order.Adjustisplain <- false
    order.Percentagevariataion <- 0.0M
    order.Plaintotalvariation <- 0.0M
    let orderItems = order.``public.orderitems by orderid``
    let recoveredTotal = orderItems  |> Seq.map (fun (x:Db.OrderItem) -> x.Price) |> Seq.fold (+) (decimal 0.0)
    order.Adjustedtotal <- recoveredTotal
    ctx.SubmitUpdates()
    Redirection.FOUND Path.Orders.seeDoneOrders


let unVoidLatestVoided backUrl (user:UserLoggedOnSession)  =
    log.Debug(sprintf "%s\n" "unVoidLatestVoided")
    let ctx = Db.getContext()
    let myLatestVoidedOrder = Db.getLatestVoidedOrder user.UserId ctx
    match myLatestVoidedOrder with
    | Some theLatestVoideOrderByMe ->
        let order = Db.Orders.getOrder theLatestVoideOrderByMe.Orderid ctx
        order.Voided <- false
        theLatestVoideOrderByMe.Delete()
        ctx.SubmitUpdates()
    | _ -> ()
    Redirection.found (WebUtility.UrlDecode backUrl)

type mymodel = {content:string;table: string}

let qrUserImageGen (user:UserLoggedOnSession) = 
    log.Debug("qrUserImageGen")
    warbler (fun x -> 
        let urlToCode = match x.request.queryParam("qrUserLoginUrl") with | Choice1Of2 par -> System.Net.WebUtility.UrlDecode par | _ -> "error"
        let table = match x.request.queryParam("table") with | Choice1Of2 par -> par | _  -> "" 
        let qrGenerator = new QRCoder.QRCodeGenerator();
        let qrCodeData  = qrGenerator.CreateQrCode(urlToCode,QRCodeGenerator.ECCLevel.Q)
        let qrCode = new QRCode(qrCodeData);
        let qrCodeImage = qrCode.GetGraphic(20)
        let stream = new System.IO.MemoryStream()
        qrCodeImage.Save(stream,qrCodeImage.RawFormat)
        let arrayOfQrCode = stream.ToArray()
        let encoded = System.Convert.ToBase64String (arrayOfQrCode)
        let o = {content=encoded; table=table}
        DotLiquid.page("qrCode.html") o
    ) 



let stateGroupIdentifierMappingForImportedOrderItems (orderItems: OrderItemDetailsWrapped list) =
    let ctx = Db.getContext()
    let differentStates  = orderItems |> List.map (fun (x:OrderItemDetailsWrapped) -> x.Stateid)
    let setOfDifferentStates = differentStates |> Set.ofList
    let n = Set.count setOfDifferentStates 
    let availableGroupIdentifiers = Db.getFirstNAvailableOutGroupIdentifier n ctx
    let stateGroupMapping = List.zip (setOfDifferentStates |> Set.toList) availableGroupIdentifiers |> Map.ofList
    stateGroupMapping


let selectOrderFromWhichMoveOrderItems targetOrderId (user:UserLoggedOnSession) =
    log.Debug("selectOrderFromWhichMoveOrderItems")
    let ctx = Db.getContext() 
    let orders = Db.Orders.getAllUnarchivedOrders ctx
    let wrappedOrders = orders |> List.filter (fun (x:Db.Order) -> x.Orderid <> targetOrderId) |> List.map (fun (x:Db.Order) -> DbWrappedEntities.DbObjectWrapper.WrapOrder x) 
    let targetOrder = Db.Orders.getOrder targetOrderId ctx
    let wrappedTargetOrder = DbWrappedEntities.DbObjectWrapper.WrapOrder targetOrder
    let liquidModel = {orders = wrappedOrders ;  targetOrder = wrappedTargetOrder}

    choose [
        GET >=>  warbler (fun (req:HttpContext) ->
            let expectedOrderParameterNames = wrappedOrders |> List.map (fun (x:OrderWrapped) -> "order" + (x.OrderId|> string)) // form: order999 in querystring
            let orderIdsTomerge = expectedOrderParameterNames |> List.filter (fun z -> match (req.request.queryParam(z)) with | Choice1Of2 _ -> true | _ -> false ) |> List.map (fun y -> (y.Substring("order".Length) |> int)) // list of ids of orders

            let wrapperOrdersToMerge = wrappedOrders |> List.filter (fun x -> List.contains x.OrderId orderIdsTomerge ) // a wrappedorder format for orders to merge

            let orderAndOrderItems = wrapperOrdersToMerge |> 
                List.map (fun (x:OrderWrapped) -> {order=x; orderitems =  (Db.getUnpaidOrderItemDetailOfOrderById x.OrderId ctx) |> 
                    List.map (fun (x:Db.OrderItemDetails) -> (DbWrappedEntities.DbObjectWrapper.WrapOrderItemDetails x "0000"))})

            let liquidTableMergeModel = { orderandorderitems=orderAndOrderItems; targetorder = wrappedTargetOrder  }

            let expectedOrderItems = orderAndOrderItems |> List.map (fun (x:orderandorderitemslist) -> x.orderitems) |> List.fold (@) []    // |> List.map (fun (y:OrderItemDetailsWrapped) -> y.Orderitemid))) |> List.fold(@) []   I
            let expectedOrderItemsParameterNames = expectedOrderItems |> List.map (fun (x:OrderItemDetailsWrapped) -> "orderitem"+(x.Orderitemid|> string) )

            let orderItemsIdsToMerge = expectedOrderItemsParameterNames |> List.filter (fun z -> match (req.request.queryParam(z)) with | Choice1Of2 _ -> true| _ -> false  ) |> List.map (fun y -> (y.Substring("orderitem".Length) |> int ))

            let wrappedOrderItemsToMerge = expectedOrderItems |> List.filter (fun x -> List.contains x.Orderitemid orderItemsIdsToMerge )

            let stateNewTargetGroupsMapping = stateGroupIdentifierMappingForImportedOrderItems wrappedOrderItemsToMerge 

            match ((List.length wrapperOrdersToMerge),wrappedOrderItemsToMerge) with 
                | (_,Y) when (List.length Y > 0) -> 

                                let _ = Y |> List.iter (fun (x:OrderItemDetailsWrapped) -> 
                                        let outGroup = Db.createOrGetOutGroup targetOrderId (stateNewTargetGroupsMapping.[x.Stateid]) ctx
                                        let _ = if (not (Db.isInitialState x.Stateid ctx)) then outGroup.Printcount <- outGroup.Printcount + 1
                                        Db.tryMoveOrderItemToAnOutGroupOfAnotherOrder x.Orderitemid outGroup.Ordergroupid ctx )
                                Redirection.found (sprintf Path.Orders.viewOrder targetOrderId)

                | (X,_) when (X > 0) -> DotLiquid.page("selectOrderItemsToMerge.html") liquidTableMergeModel
                | _ ->  DotLiquid.page("selectOrderToMerge.html") liquidModel

        )
    ]

let noCache = 
  setHeader "Cache-Control" "no-cache, no-store, must-revalidate"
  >=> setHeader "Pragma" "no-cache"
  >=> setHeader "Expires" "0"

let webPart =
    choose [ 

        pathScan Path.Orders.removeAllDiscountOfSubOrder (fun id -> admin (removeAllDiscountOfSubOrder id))
        pathScan Path.Orders.removePaymentItemOfOrder (fun (paymentId,orderId) -> admin (removePaymentItemOfOrder paymentId orderId) )
        pathScan Path.Orders.createEcrReceiptInstructionForSubOrder (fun (suborderid,orderid) -> admin (createEcrReceiptInstructionForSubOrder suborderid orderid))
        pathScan Path.Orders.createEcrReceiptInstructionForOrder (fun orderid -> admin (createEcrReceiptInstructionForOrder orderid))
        pathScan Path.Orders.removePaymentItemOfSubOrder (fun (paymentId,subOrderId,orerId) -> adminPassingUserLoggedOn (removePaymentItemOfSubOrder paymentId subOrderId orerId))
        pathScan Path.Orders.selectOrderFromWhichMoveOrderItems  (fun targetOrerId -> anyUserExceptTemporary   (selectOrderFromWhichMoveOrderItems targetOrerId))
        pathScan Path.Orders.wholeOrderPaymentItems (fun orderId -> admin (wholeOrderPaymentItems orderId))
        pathScan Path.Orders.subOrderPaymentItems (fun (subOrderId,orderId) -> adminPassingUserLoggedOn (subOrderPaymentItems subOrderId orderId))
        pathScan Path.Orders.viewOrder (fun id -> anyUserPassingUserLoggedOn (viewSingleOrder id))
        path Path.Extension.qrUserImageGen >=> adminPassingUserLoggedOn qrUserImageGen
        path Path.Extension.qrCodeLogin >=> logonViaQrCode
        path Path.Extension.addQruser >=> admin addQrUser
        pathScan Path.Extension.regenTempUser  (fun id -> admin (regenTempUser id))
        pathScan Path.Orders.removeAllAllergenicFromOrderItem (fun (orderItemId,encodedBackUrl) -> anyUserPassingUserLoggedOn (removeAllAllergenic orderItemId encodedBackUrl))
        pathScan Path.Orders.removeAllUnavailableIngredientsFromOrderItem (fun (orderItemId,encodedBackUrl) -> anyUserPassingUserLoggedOn (removeAllInvisibleIngredients orderItemId encodedBackUrl))
        pathScan Path.Orders.rejectOrderItem (fun orderItemId  -> anyUserPassingUserLoggedOn (rejectOrderItem orderItemId))
        pathScan Path.Orders.printWholeOrderReceipt (fun orderId -> admin (printWholeOrderReceipt orderId))
        pathScan Path.Orders.printWholeOrderInvoice (fun orderId -> admin (printWholeOrderInvoice orderId))
        pathScan Path.Orders.unVoidLatestVoidedOrderOfUser (fun (userId,encodedBackUrl) -> anyUserPassingUserLoggedOn (unVoidLatestVoided encodedBackUrl))
        path Path.Orders.dearchiveLatestOrder >=> anyUserPassingUserLoggedOn deArchiveLatestOrder
        pathScan Path.Admin.removePrinter (fun id -> admin (deletePrinter id))
        pathScan Path.Admin.deleteIngredientPrice (fun id -> admin (deleteIngredientPrice id))
        pathScan Path.Admin.deleteIngredient (fun id -> admin (deleteIngredient id))
        path Path.Admin.deleteIngredients >=> canManageIngredients deleteIngredientsBySelection
        pathScan Path.Admin.deleteRole (fun id -> admin (deleteUserRole id))
        path Path.Admin.deleteUserRoles >=> admin deleteUserRoles
        pathScan Path.Admin.deleteCourseCategory (fun id -> admin (deleteCourseCategory id))
        path Path.Admin.deleteCourseCategories >=> canManageCourses deleteCourseCategories
        path Path.Admin.deleteIngredientCategories >=> canManageIngredients deleteIngredientCategories
        path Path.Admin.deleteCourses >=> canManageCourses deleteCourses
        pathScan Path.Admin.deleteIngredientCategory (fun id -> admin (deleteIngredientCategory id))
        pathScan Path.Admin.deleteUser (fun id -> admin (deleteUser id))
        path Path.Admin.deleteUsers >=> admin deleteUsers
        path Path.Admin.deleteTemporaryUsers >=> admin deleteTemporaryUsers
        path Path.Admin.deleteObjects >=> admin deleteObjects
        path Path.Admin.optimizeVoided >=> admin  optimizeVoided
        pathScan Path.Orders.editOrderItemVariation ( fun (orderItemId,encodedBackUrl) -> anyUserPassingUserLoggedOn (editOrderItemVariationPassingUserLoggedOn orderItemId encodedBackUrl))
        pathScan Path.Orders.editOrderItemVariationByIngredientCategory ( fun (orderItemId,categoryId,encodedBackUrl) -> anyUserPassingUserLoggedOn (editOrderItemVariationByIngredientCategoryPasssingUserLoggedOn orderItemId categoryId encodedBackUrl))
        pathScan Path.Orders.removeIngredientVariation (fun (variationId, orderItemId,encodedBackUrl) ->  anyUserPassingUserLoggedOn (removeIngredientVariation variationId orderItemId encodedBackUrl)  )
        pathScan Path.Orders.increaseUnitaryIngredientVariation (fun (variationId,encodedBackUrl) -> anyUserPassingUserLoggedOn (increaseUnitaryIngredientVariation variationId encodedBackUrl))
        pathScan Path.Orders.decreaseUnitaryIngredientVariation (fun (variationId,encodedBackUrl) -> anyUserPassingUserLoggedOn (decreaseUnitaryIngredientVariation variationId encodedBackUrl))
        pathScan Path.Orders.addIncreaseIngredientVariation (fun (orderItemId,ingredientId,encodedBackUrl) -> anyUserPassingUserLoggedOn (addIncreaseIngredientVariation orderItemId ingredientId encodedBackUrl))
        pathScan Path.Orders.addDiminuishIngredientVariation (fun (orderItemId,ingredientId,encodedBackUrl) -> anyUserPassingUserLoggedOn (addDiminuishIngredientVariation orderItemId ingredientId encodedBackUrl ))
        pathScan Path.Orders.addWithougIngredientVariation (fun (orderItemId,ingredientId,ingredientcourseId, encodedBackUrl)-> anyUserPassingUserLoggedOn (addWithoutIngredientVariation orderItemId ingredientId encodedBackUrl))
        pathScan Path.Courses.deleteIngredientToCourse (fun (courseId, ingredientId) -> canManageCourses (deleteIngredientToCourse courseId ingredientId))
        pathScan Path.Courses.selectAllIngredientsForCourseEdit (fun courseId -> canManageCourses (selectIAllngredientCatForCourse courseId))
        pathScan Path.Courses.selectIngredientCategoryForCourseEdit (fun (courseId,categoryId) -> canManageCourses (selectIngredientCatForCourse courseId categoryId "" ) )
        pathScan Path.Admin.editIngredient (fun (id,pageNumberBack) -> admin (editIngredient id pageNumberBack))
        pathScan Path.Admin.viewIngredientUsage (fun (ingredientId,pageNumberBack) -> canManageCourses (viewIngredientUsage ingredientId pageNumberBack))
        pathScan Path.Admin.fillIngredient (fun (id,pageNumberBack) -> canManageIngredientsPassingUserLoggedOn (fillIngredient id pageNumberBack))
        pathScan Path.Admin.switchVisibilityOfIngredientCategory (fun (id,encodedBackUrl) -> canManageIngredients (switchVisbilityOfIngredientCategory id encodedBackUrl))
        pathScan Path.Admin.switchVisibilityOfIngredient (fun (idCategory,idIngredient) -> admin (switchVisbilityOfIngredient idCategory idIngredient))
        path Path.home >=> anyUserExceptTemporary controlPanel
        path Path.Orders.allOrders >=> adminPassingUserLoggedOn allOrders // >=> noCache
        path Path.Extension.qrUserOrder >=> temporaryUserPassingUserLoggedOn qrOrder
        path Path.Orders.myOrders  >=>  anyUserPassingUserLoggedOn myOrders // >=> noCache
        path Path.Orders.myOrdersSingles  >=>  anyUserPassingUserLoggedOn myOrdersSingles // >=> noCache
        path Path.Orders.addOrder >=> anyUserPassingUserLoggedOn createOrderByUserLoggedOn
        path Path.Orders.addSingleOrder >=> anyUserPassingUserLoggedOn createSingleOrderByUserLoggedOn
        pathScan Path.Orders.addOrderItemByCategory  (fun (idOrder,idCategory, urlEncodedBackUrl) -> anyUserPassingUserLoggedOn (addOrderItemByCategoryPassingUserLoggedOn idOrder idCategory urlEncodedBackUrl))
        pathScan Path.Orders.resetVariationsAndEditOrderItemByCategory (fun (idOrder,idCategory,urlEncodedBackUrl) ->    anyUserPassingUserLoggedOn (resetVariationThenEditOrderItemByCat idOrder idCategory Path.Orders.myOrders urlEncodedBackUrl) )
        pathScan Path.Orders.editOrderItemByCategory (fun (idOrder,idCategory,encodedBackUrl) -> anyUserPassingUserLoggedOn (editOrderItemByCategoryPassingUserLoggedOn idOrder idCategory Path.Orders.myOrders encodedBackUrl))
        pathScan Path.Orders.editDoneOrderitem (fun (idOrder,idCategory) -> anyUserPassingUserLoggedOn (editOrderItemByCategoryPassingUserLoggedOn idOrder idCategory Path.Orders.seeDoneOrders Path.Orders.seeDoneOrders))
        pathScan Path.Orders.resetDiscount (fun idOrder -> admin (resetDiscount idOrder))
        pathScan Path.Orders.editDoneOrderItemFromSubSplitting (fun (orderItemId,idCategory,orderId) -> anyUserPassingUserLoggedOn (editOrderItemByCategoryPassingUserLoggedOn orderItemId idCategory (sprintf Path.Orders.subdivideDoneOrder orderId) (sprintf Path.Orders.subdivideDoneOrder orderId)))
        pathScan Path.Orders.achiveOrder  (fun id -> anyUserPassingUserLoggedOn (archiveOrder id))
        path Path.Orders.seeDoneOrders >=> anyUserPassingUserLoggedOn seeDoneOrders
        pathScan Path.Orders.moveAllInitialStateOrderItems (fun (orderId) -> anyUserPassingUserLoggedOn (moveAllOrderItemsInBlockFromInitialState orderId))
        pathScan Path.Orders.moveInitialStateOrderItemsByOutGroup (fun (orderId,groupId,encodedBackUrl) -> anyUserPassingUserLoggedOn (moveInitialStateOrderItemsByOrderOutGroup orderId groupId encodedBackUrl))
        pathScan Path.Orders.reprintOrderItemsGroup (fun (orderId,groupId,encodedBackUrl) -> anyUserPassingUserLoggedOn (rePrintOrderOutGroup orderId groupId encodedBackUrl))
        pathScan Path.Orders.moveOrderItemToTheNextStateAndGoMyOrders (fun (itemId,encodedBackUrl) -> anyUserPassingUserLoggedOn (moveOrderItemNextStepAndBacktoOrder itemId encodedBackUrl ))
        pathScan Path.Orders.removeOrderItemThenGoBackToUrl (fun (itemId,encodedBackUrl) -> adminPassingUserLoggedOn (removeOrderItemThenGoBackToUrl itemId encodedBackUrl ))
        pathScan Path.Orders.moveOrderItemToTheNextStateAndGoOrdersProgress (fun itemId -> anyUserPassingUserLoggedOn (moveOrderItemNextStep itemId Path.Orders.orderItemsProgress ))
        pathScan Path.Orders.confirmVoidOrderFromMyOrders (fun (id,encodedBackurl) -> anyUserPassingUserLoggedOn (voidOrderByUserLoggedOn id encodedBackurl ))
        pathScan Path.Orders.voidOrderFromMyOrders (fun (id,encodedBackurl) -> anyUserPassingUserLoggedOn (askConfirmationVoidOrderByUserLoggedOn id encodedBackurl ))
        pathScan Path.Courses.editCategory  (fun id -> canManageCourses (editCourseCategory id))
        pathScan Path.Orders.deleteOrderItem (fun (id,encodedBackUrl) -> anyUserPassingUserLoggedOn (deleteOrderItemByUser id encodedBackUrl))
        pathScan Path.Admin.editUser (fun id -> admin (editUser id))
        pathScan Path.Admin.editTemporaryUser (fun id -> admin (editTemporaryUser id))
        path Path.Account.logoff >=> reset
        path Path.Account.logon >=> logon
        path Path.Errors.unableToCompleteOperation >=> unableToCompleteOperation "an error occurred "
        path Path.Courses.manageAllCourses >=> admin manageCourses
        path Path.Courses.addCategory >=> canManageCourses createCategory
        pathScan Path.Courses.manageVisibleCoursesOfACategory  (fun id -> admin (manageVisibleCoursesOfACategory id))
        pathScan Path.Courses.manageAllCoursesOfACategory  (fun id -> admin (manageAllCoursesOfACategory id))
        pathScan Path.Courses.manageAllCoursesOfACategoryPaginated (fun (categoryId,pageNumber) -> canManageCourses (manageAllCoursesOfACategoryPaginated categoryId pageNumber ))
        pathScan Path.Courses.manageVisibleCoursesOfACategoryPaginated (fun (categoryId,pageNumber) -> canManageCourses (manageVisibleCoursesOfACategoryPaginated categoryId pageNumber ))
        pathScan Path.Courses.switchCourseCategoryVisibility (fun categoryId -> admin (switchCourseCategoryVisibility categoryId ))
        path Path.Courses.manageAllCategories >=> canManageCourses manageCategories
        pathScan Path.Courses.editCourse (fun id -> canManageCourses (editCourse id))
        pathScan Path.Courses.addCourseByCategory (fun id -> canManageCourses (createCourseByCatgory ((decimal)id)))
        path Path.Courses.adminCategories >=> canManageCourses adminCategories 
        path Path.Admin.allUsers >=> admin ordinaryUsers
        path Path.Admin.temporaryUsers >=> admin temporaryUsers
        path Path.Admin.addUser >=> admin addUser
        path Path.Admin.roles  >=> admin adminRoles
        path Path.Admin.allIngredientCategories >=> canManageIngredients adminIngredientCategories
        path Path.Admin.recognizePrinters >=> admin  recognizePrinters
        pathScan Path.Admin.managePrinter (fun (printerId,categoryId) -> admin (managePrinter printerId categoryId))
        path Path.Admin.resetPrinters >=> admin resetPrinters
        path Path.Admin.info >=> admin about
        path Path.Admin.printers >=> admin adminPrinters
        path Path.Admin.visibleIngredientCategories >=> canManageIngredients visibleingredientCategories
        pathScan Path.Admin.editIngredientCategory (fun id -> (canManageIngredientsPassingUserLoggedOn (editIngredientCategory id)))
        pathScan Path.Admin.editIngredientPrices  (fun id -> admin  (editIngredientPrices id))
        pathScan Path.Admin.editIngredientCategoryPaginated (fun (categoryid, pageNumber) -> (canManageIngredients (editIngredientCategoryPaginated categoryid pageNumber)))
        path Path.Admin.addRole >=> adminPassingUserLoggedOn createRole
        path Path.Admin.roleEnablerObserverCategoriesByCheckBoxes >=> admin roleEnablerObserverCategoriesByCheckBoxes
        pathScan Path.Admin.roleEnablerObserverCategoriesByCheckBoxesByRoleAndCat (fun (roleId,catId) -> admin (roleEnablerObserverCategoriesByCheckBoxesWithRoleAndCat roleId catId))
        pathScan Path.Admin.setStateAsActionableForSpecificWaiter  (fun (userId, stateId) -> admin (setStateAsActionableForWaiter userId stateId))
        pathScan Path.Admin.unSetStateAsActionableForSpecificWaiter  (fun (userId, stateId) -> admin (unSetStateAsAcionableForWaiter userId stateId))
        path Path.Admin.defaultActionableStatesForOrderOwner >=> admin defaultActionableStatesForOrderOwner
        path Path.Admin.tempUserDefaultActionableStates >=> admin tempUserDefaultActionableStates
        pathScan Path.Admin.actionableStatesForSpecificOrderOwner  (fun id -> admin (specificActionableStateForOrderOwner id))
        pathScan Path.Admin.setStateAsActionableForWaiterByDefault (fun id -> admin (setStateAsActiobableByDefault id))
        pathScan Path.Admin.setStateAsActionableForTempUserByDefault (fun id -> admin (setStateAsActionableForTempUserByDefault id))
        pathScan Path.Admin.unSetStateAsActionableForWaiterByDefault (fun id -> admin (unSetStateAsActiobableByDefault id))
        pathScan Path.Admin.unSetStateAsActionableForTempUserByDefault (fun id -> admin (unSetStateAsActionableForTempUserByDefault id))
        pathScan Path.Admin.removeObserverMapping (fun id -> admin (deleteObserverMapping id))
        pathScan Path.Admin.removeEnablerMapping (fun id -> admin (deleteEnablerMapping id))
        path Path.Orders.orderItemsProgress >=> anyUserPassingUserLoggedOn orderItemProgress 
        path Path.Account.changePassword >=> anyUserPassingUserLoggedOn changePassword
        pathScan Path.Orders.editDoneOrder (fun id -> adminPassingUserLoggedOn (editDoneOrder id))
        pathScan Path.Orders.subdivideDoneOrder  (fun id -> adminPassingUserLoggedOn (subdivideDoneOrder id))
        pathScan Path.Orders.colapseDoneOrder  (fun id -> adminPassingUserLoggedOn (colapseDoneOrder id))
        pathScan Path.Orders.splitOrderItemInToUnitaryOrderItems (fun id -> adminPassingUserLoggedOn (splitOrderItemInToUnitaryOrderItems id))
        pathScan Path.Orders.deleteSubOrder (fun (subOrderId,orderId) -> anyUserPassingUserLoggedOn (deleteSubOrder subOrderId orderId))
        pathScan Path.Orders.printReceipt (fun (subOrderId,orderId) -> admin (printReceipt subOrderId orderId))
        pathScan Path.Orders.printSubOrderInvoice (fun subOrderId -> admin (printSubOrderInvoice subOrderId))
        pathScan Path.Orders.setSubOrderAsPaid (fun (subOrderId,orderId) -> adminPassingUserLoggedOn (setSubOrderAsPaid subOrderId orderId))
        pathScan Path.Orders.setSubOrderAsNotPaid (fun (subOrderId,orderId) -> adminPassingUserLoggedOn (setSubOrderAsNotPaid subOrderId orderId))
        pathRegex "(.*)\.(css|png|gif|js|html)" >=> Files.browseHome
        html View.notFound

    ]



// let cert = new X509Certificate2("certificate.p12","secret")

let cfg =
  { defaultConfig with
      bindings = [ HttpBinding.createSimple HTTP "0.0.0.0" 8083  ] 
    //    bindings = [ HttpBinding.createSimple (HTTPS cert) "0.0.0.0" 8443  ] 
  }

DotLiquid.setTemplatesDir ("templates-"+Settings.Localization)


startWebServer cfg webPart
