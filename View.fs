module OrdersSystem.View

open Suave.Form
open Suave.Html
open OrdersSystem
open OrdersSystem.Db
open OrdersSystem.Globals
open OrdersSystem.Form
open System.Web
open System.Net
open UIFragments
open System.Net
open OrdersSystem.DataProvider


let local = Resource.Load ("resources-"+Settings.Localization+".xml")

let visibilityType = [("VISIBLE","VISIBLE");("INVISIBLE","INVISIBLE")]
let abstractType = [("ABSTRACT","ABSTRACT");("CONCRETE","CONCRETE")]

let percentageOrValue = [("PERCENTUALE","PERCENTUALE");("VALORE","VALORE")]

let pocoNormaleMolto = [(Globals.AGGIUNGIPOCO,"POCO");(Globals.AGGIUNGINORMALE,"NORMALE");(Globals.AGGIUNGIMOLTO,"MOLTO")]

let yesOrNo = [("NO","NO");("YES","YES")]

let visibilityTypeBool = [(true,"VISIBLE");(false,"INVISIBLE")]

let enabledType = [("ENABLED","ENABLED");("DISABLED","DISABLED")]

let unit_of_measures_drop_box = (Globals.WEIGHT_UNIT_OF_MEASURES @ Globals.LIQUID_UNIT_OF_MEASURES @ UNITARY_MEASURES) |> List.map (fun x -> (x,x))

type UserLoggedOnSession = {
    Username : string
    Role : string
    UserId : int
    RoleId : int
    Temporary: bool
    CanManageAllCourses: bool
    CanManageAllOrders: bool
}

let isUserAdmin(user:UserLoggedOnSession) = 
    user.Role = "admin"

type Session = 
   | NoSession
   | CartIdOnly of string
   | UserLoggedOn of UserLoggedOnSession


let editTemporaryUser (user:Db.User) (host:string) = 
 let qrUserLoginUrl = "http://"+host+Path.Extension.qrCodeLogin+"?specialCode="+user.Username+"&returnPath="+Path.Extension.qrUserOrder
 

 let linkImageGen = tag "p" [] [a (Path.Extension.qrUserImageGen+"?qrUserLoginUrl="+System.Net.WebUtility.UrlEncode(qrUserLoginUrl)+"&table="+user.Table)  [("target","_blink");("class","buttonX")] [Text("genera codice di accesso q ")]]
 [
  
    h2 ("Edit user: " + user.Username)
    p [("id","myUrl")] [Text(qrUserLoginUrl)]
    tag "copia url" [("id","button");("onclick","copyToClipboard('#myUrl')")] [Text(local.Button)]

    br []
    br []

    linkImageGen

    div [] [a (Path.Admin.temporaryUsers)   [] [Text(local.GoBack)]]
    script ["type","text/javascript"; "src","/jquery-3.1.1.min.js"] []

    script ["type","text/javascript"; "src","/copytoclipboard.js"] []

]

let editUser (user:Db.User) = [
    h2 ("Edit user: " + user.Username)
    renderForm 
       { Form = Form.userEdit
         Fieldsets = 
             [ { Legend = local.ModifyUser
                 Fields = 
                     [ 
                       { Label = local.UserEnabled
                         Html = selectInput (fun f -> <@ f.Enabled  @>)  yesOrNo (if (user.Enabled) then  Some "YES" else Some "NO"  )
                       }
                       { Label = local.UserCanVoidOrder
                         Html = selectInput (fun f -> <@ f.CanVoidOrder  @>)  yesOrNo (if (user.Canvoidorders) then Some "YES"  else Some "NO" )
                       }
                       { Label = local.UserCanSeeAnyOrder
                         Html = selectInput (fun f -> <@ f.CanManageAllorders  @>)  yesOrNo (if (user.Canmanageallorders) then Some "YES"  else Some "NO" )
                       }
                       { Label = local.UserCanChangePrices
                         Html = selectInput (fun f -> <@ f.CanChangeThePrices  @>)  yesOrNo (if (user.Canchangetheprice) then Some "YES"  else Some "NO" )
                       }
                       { Label = local.UserCanManageCourses
                         Html = selectInput (fun f -> <@ f.CanManageAllCourses  @>)  yesOrNo (if (user.Canmanagecourses) then Some "YES"  else Some "NO" )
                       }
                       
                     ] 
                } 
             ]
         SubmitText = local.Modify }
]


let editCourseCategory  (courseCategory: Db.CourseCategories) message = [
   h2 local.EditCategory

   div ["id", "register-message"] [
               Text message
   ]

   renderForm
       { Form = Form.courseCategoryEdit
         Fieldsets = 
             [ { Legend = local.Course
                 Fields = 
                     [ 
                       { Label = local.Name
                         Html =  formInput 
                                   (fun f -> <@ f.Name  @>) 
                                   ["value", courseCategory.Name] }
                       { Label = local.Visibility
                         Html = Suave.Form.selectInput (fun f -> <@ f.Visibility @>) visibilityType
                          (match courseCategory.Visibile with true -> Some "VISIBLE" | _ -> Some "INVISIBLE" ) 
                         } 
                       { Label = local.Abstract
                         Html = Suave.Form.selectInput (fun f -> <@ f.Abstract @>) abstractType
                          (match courseCategory.Abstract with true -> Some "ABSTRACT" | _ -> Some "CONCRETE" ) 
                         } 

                      ] 
               } 
             ]
         SubmitText = local.SaveChanges }
         
   br []
   div [] [
       a Path.Courses.adminCategories [] [Text local.GoBack]
   ]
]

let addIngredientToCourse (selectableIngredients:Db.Ingredient list) (course:Db.Course) message = 
 let ingredientsIdNameMap = selectableIngredients |> List.map (fun (x:Db.Ingredient) -> ((decimal)x.Ingredientid,x.Name))

 [
    h2 (local.AddIngredientTo + course.Name)

    div ["id", "register-message"] [
        
               Text message
    ]

    renderForm

       { Form = Form.ingredientSelector
         Fieldsets = 
             [ { Legend = local.Ingredient
                 Fields = 
                     [ 
                       { Label = local.NameBySelection
                         Html =  selectInput 
                                   (fun f -> <@ f.IngredientBySelect  @>) 
                                    ingredientsIdNameMap (None)}
                       { Label = local.NameByFreeText
                         Html =  formInput 
                                   (fun f -> <@ f.IngredientByText  @>) []}

                       { Label = local.Quantity
                         Html =  formInput 
                                   (fun f -> <@ f.Quantity  @>) 
                                     [] }

                                   ] } 
                                   ]
         SubmitText = local.Add }

    script ["type", "text/javascript"; "src", "/jquery-3.1.1.min.js" ] []
    script ["type","text/javascript"; "src","/autocompleteIng.js"] []
    
]
                    


let editCourse  (course : Db.Course) courseCategories  (ingredientCategories:Db.IngredientCategory list) 
      (ingredientsOfTheCourse:Db.IngredientOfCourse list)
     message = [ 

   h2 "Edit"

   div ["id", "register-message"] [
               Text message
   ]

   renderForm
       { Form = Form.course
         Fieldsets = 
             [ { Legend = local.Course
                 Fields = 
                     [ 
                       { Label = local.Name
                         Html =  formInput 
                                   (fun f -> <@ f.Name  @>) 
                                   ["value", course.Name] }
                       { Label = local.Price
                         Html = formInput 
                                   (fun f -> <@ f.Price @>) 
                                   ["value", formatDec course.Price ] }
                       { Label = local.Description
                         Html = formInput 
                                   (fun f -> <@ f.Description @>) 
                                   ["value", course.Description ] } 
                       { Label = local.Visibility
                         Html = selectInput (fun f -> <@ f.Visibile @>) visibilityType (Some (if course.Visibility then "VISIBLE" else "INVISIBLE")) } 
                       { Label = local.Category
                         Html = selectInput (fun f -> <@ f.CategoryId @>) courseCategories (Some ((decimal)course.Categoryid))  } 

                                   ] } 
                                   ]
         SubmitText = local.SaveChanges }
   br []

   h2 local.ExistingIngredients
   ulAttr ["id","item-list"] [
        for existinIngredient in ingredientsOfTheCourse ->
            tag "p" [] [
                 a (sprintf Path.Courses.deleteIngredientToCourse  course.Courseid existinIngredient.Ingredientid) ["class","buttonX"] 
                  [Text (existinIngredient.Ingredientname +  (if (existinIngredient.Quantity<> (decimal)0.0) then (sprintf "%s %2f" local.Quantity existinIngredient.Quantity) else "")+  " (rimuovi)")] 

            ]
   ]

   br []

   h2 local.CategoriesOfIngredientsYouCanAdd
   ulAttr ["id","item-list"] [
        for ingredientCategory in ingredientCategories ->

                tag "p" [] [
                    a (sprintf Path.Courses.selectIngredientCategoryForCourseEdit  
                        course.Courseid ingredientCategory.Ingredientcategoryid) ["class","buttonX"] 
                        [Text (local.AddAmong+" "+ingredientCategory.Name)] 
                ]
   ]

   tag "p" [] [
        a (sprintf Path.Courses.selectAllIngredientsForCourseEdit course.Courseid)   ["class","buttonX"] [Text (local.AddAmongAll) ] 
   ]

   tag "button" [("onclick","goBack()");("class","buttonX")] [Text(local.GoBack)]

   div [] [
       a Path.home [] [Text local.MainPage ]
   ]
   script ["type","text/javascript"; "src","/backbutton.js"] []
]


let partUser (user : string option) = 
    div ["id", "part-user"] [
        match user with
        | Some user -> 
            yield Text (sprintf "Logged on as %s, " user)
            yield a Path.Account.logoff [] [Text "Log off"]
        | None ->
            yield a Path.Account.logon [] [Text "Log on"]
    ]

let index container = 
 "<!DOCTYPE html><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">"+
    (html [] [
        head [] [
            title [] "Orders system"
            cssLink "/Site.css?ZkasaasdfsdXXyty"
        ]
        body [] [
            div ["id", "header"] [
                tag "h1" [] [
                    a Path.home [] [Text "Orders System"]
                    br []
                ]
            ]

            div ["id", "main"] container

            div ["id", "footer"] [
                strong "made by tonyx1@gmail.com"
                Text " with "
                a "http://suave.io" [] [Text "Suave.IO"]
            ]
        ]
    ]
    |> htmlToString)


let changePassword message (user:UserLoggedOnSession) =
        [
        h2 (local.ChangePasswordFor + user.Username)
        p [] [
            Text local.ChangePassword
        ]

        div ["id", "register-message"] [
               Text message
        ]

        renderForm

         { Form = Form.changePassword
           Fieldsets = 
                     [ { Legend = local.ChangePassword
                         Fields = 
                             [ 
                               { Label = local.OldPassword
                                 Html = formInput (fun f -> <@ f.OldPassword @>) [] }
                               { Label = "Password "+local.SixToTwentyChars
                                 Html = formInput (fun f -> <@ f.Password @>) [] }
                               { Label = local.ConfirmPassword
                                 Html = formInput (fun f -> <@ f.ConfirmPassword @>) [] }
                              ] } ]
           SubmitText = "Confirm" }

    ]


let register (roles: Db.Role list)   msg = 
   let rolesIdAndNames = List.map (fun (x:Db.Role) -> ((decimal)x.Roleid,x.Rolename)) roles

   [
   h2 local.CreateNewAccount
   p [] [
       Text local.FillData
   ]

   div ["id", "register-message"] [
       Text msg
   ]

   renderForm
       { Form = Form.register
         Fieldsets = 
             [ { Legend=  local.CreateNewAccount
                 Fields = 
                     [ { Label = "User name (max 30 characters)"
                         Html = formInput (fun f -> <@ f.Username @>) [] }
                       { Label = local.Role
                         Html = selectInput (fun f -> <@ f.Role @>) rolesIdAndNames (None) }
                       { Label = local.CanSeeAllOrders
                         Html = selectInput (fun f -> <@ f.CanManageAllorders @>) yesOrNo  (Some "No") }
                       { Label = local.CanOverWritePrices
                         Html = selectInput (fun f -> <@ f.CanChangeThePrices @>) yesOrNo  (Some "No") }
                       { Label = local.UserCanChangePrices
                         Html = selectInput (fun f -> <@ f.CanManageAllCourses @>) yesOrNo  (Some "No") }
                       { Label = local.SixToTwentyChars 
                         Html = formInput (fun f -> <@ f.Password @>) [] }
                       { Label = local.ConfirmPassword
                         Html = formInput (fun f -> <@ f.ConfirmPassword @>) [] }
                       ] } ]
         SubmitText = local.SaveChanges }
]

let notFound = [
    h2 "Pagina non trovata"
    p [] [
        Text "Could not find the requested resource"
    ]
    p [] [
        Text "Back to "
        a Path.home [] [Text "Home"]
    ]
]

let cantComplete msg = [
    h2 local.Error
    p [] [
        Text (local.AnErrorOccurred + msg)
    ]
    p [] [
        Text "Torna a "
        a Path.home [] [Text "Home"]
    ]
]

let logon msg = [
    h2 "Log On"
    div ["id", "logon-message"] [
        Text msg
    ]
    renderForm
        { Form = Form.logon
          Fieldsets = 
              [ { Legend = "Account Information"
                  Fields = 
                      [ { Label = "User Name"
                          Html = formInput (fun f -> <@ f.Username @>) [] }
                        { Label = "Password"
                          Html = formInput (fun f -> <@ f.Password @>) [] } ] } ]
          SubmitText = "Log On" 
        }
]


let addOrderItemForStrippedUsers  coursesIdWithName coursesIdWithPrices backUrl viableGroupOutIdsForOrderItem = [
    h2 local.Add
    renderForm
        { Form = Form.strippedOrderItem
          Fieldsets = 
              [ { Legend = "Order item"
                  Fields = 
                      [ 
                        { Label = local.CourseBySelection
                          Html = selectInput (fun f -> <@ f.CourseId @>) coursesIdWithName None } 
                        { Label = local.CourseByFreeText
                          Html = formInput (fun f -> <@ f.CourseByName @>) []  } 
                        { Label = local.Quantity
                          Html = formInput (fun f -> <@ f.Quantity @>) 
                           [ "Value", "1" ] } 

                        { Label = local.ExitGroup
                          Html = selectInput (fun f -> <@ f.GroupOut @>) viableGroupOutIdsForOrderItem None
                            } 

                        { Label = local.Comment
                          Html = formInput (fun f -> <@ f.Comment @>) 
                           [ ] } 
                          ] } ]
          SubmitText = local.Add }
    div [] [
        a Path.home [] [Text local.MainPage ]
    ]
    script ["type", "text/javascript"; "src", "/jquery-3.1.1.min.js" ] []
    script ["type", "text/javascript"; "src", "/script_options_stripped.js" ] []

]


let addOrderItem  coursesIdWithName coursesIdWithPrices backUrl viableGroupOutIdsForOrderItem = 

    let jsPricesForCourses = Utils.javascriptDecimalStringPairMapConverter coursesIdWithPrices 

    [
    h2 local.Add
    renderForm
        { Form = Form.orderItem
          Fieldsets = 
              [ { Legend = "Order Item"
                  Fields = 
                      [ 
                        { Label = local.CourseBySelection
                          Html = selectInput (fun f -> <@ f.CourseId @>) coursesIdWithName None } 
                        { Label = local.CourseByFreeText
                          Html = formInput (fun f -> <@ f.CourseByName @>) []  } 
                        { Label = local.Quantity
                          Html = formInput (fun f -> <@ f.Quantity @>) 
                           [ "Value", "1" ] } 
                        { Label = local.ExitGroup
                          Html = selectInput (fun f -> <@ f.GroupOut @>) viableGroupOutIdsForOrderItem None }

                        { Label = local.Comment
                          Html = formInput (fun f -> <@ f.Comment @>) 
                           [ ] } 
                        { Label = local.Price
                          Html = formInput (fun f -> <@ f.Price @>) 
                           [ ] }
                          ] } ]
          SubmitText = local.Add }
    br []
    br []
    div [] [
        a Path.home [] [Text local.MainPage]
    ]
    script ["type", "text/javascript"; "src", "/jquery-3.1.1.min.js" ] []
    script [] [Raw("var pricesForCourses = "+jsPricesForCourses)]
    script ["type","text/javascript"; "src","/script_options_add_order_item.js"] []
]


let createOrder message  = [
    h2 message
    renderForm
        { Form = Form.order
          Fieldsets = 
              [ { Legend = local.NewOrder
                  Fields = 
                      [ 
                        { Label = local.Table
                          Html = formInput (fun f -> <@ f.Table @>) [ ] } 
                        { Label = local.Person
                          Html = formInput (fun f -> <@ f.Person @>) [ ] } 
                          ] } ]
          SubmitText = local.AddOrder  }
    br []
    div [] [
        a Path.home [] [Text local.MainPage]
    ]
]

let editOrderItemForStrippedUsers (orderItem:Db.OrderItemDetails) (courses:Db.Course list)  
    (categories:Db.CourseCategories list)  (ingredients:Db.IngredientOfCourse list ) (outGroup: int) backUrl viableGroupOutIdsForOrderItem = 

    let idCoursesAndNames = List.map (fun (x:Db.Course)  -> ((decimal)x.Courseid,x.Name)) courses

    [
    Text(local.ChangeTo)

    div [] [
         for category in categories ->
            a (sprintf Path.Orders.resetVariationsAndEditOrderItemByCategory orderItem.Orderitemid category.Categoryid backUrl)  [] [Text (" "+category.Name)]
    ]

    h2 "Edit"
    renderForm
        { Form = Form.strippedOrderItem
          Fieldsets = 
              [ { Legend = "Order item"
                  Fields = 
                      [ 

                        { Label = local.CourseBySelection
                          Html = selectInput (fun f -> <@ f.CourseId @>) idCoursesAndNames (Some ((decimal)orderItem.Courseid))   } 

                        { Label = local.CourseByFreeText
                          Html = formInput (fun f -> <@ f.CourseByName @>)  []   } 

                        { Label = local.ExitGroup
                          Html = selectInput (fun f -> <@ f.GroupOut @>) viableGroupOutIdsForOrderItem (Some ((decimal)orderItem.Groupidentifier ))  } 

                        { Label = local.Quantity
                          Html = formInput (fun f -> <@ f.Quantity @>) 
                           [ "Value", (orderItem.Quantity.ToString())] } 

                        { Label = local.Comment
                          Html = formInput (fun f -> <@ f.Comment @>) 
                           [ "Value", (orderItem.Comment) ] } 

                          ] } ]
          SubmitText = local.Update }
    br []
    br []

    div [] [
        a Path.home [] [Text local.MainPage]
    ]
    script ["type", "text/javascript"; "src", "/jquery-3.1.1.min.js" ] []
    script ["type", "text/javascript"; "src", "/script_options_stripped.js" ] []
]
let editOrderItemForOrdinaryUsers (orderItem:Db.OrderItemDetails) 
    (courses:Db.Course list)  (categories:Db.CourseCategories list)  
    (ingredients:Db.IngredientOfCourse list ) (outGroup: int) backUrl viableGroupOutIdsForOrderItem = 

    let idCoursesAndNames = List.map (fun (x:Db.Course)  -> ((decimal)x.Courseid,x.Name)) courses
    let pricesForCourses = (List.map(fun (x:Db.Course) -> ((decimal) x.Courseid,   x.Price   |> string)) courses) //@ [((decimal) -1,(string)orderItem.Price)] // meant to remember old overwriten price

    let jsPricesForCourses = Utils.javascriptDecimalStringPairMapConverter pricesForCourses 

    [
    Text(local.ChangeTo)
    div [] [
         for category in categories ->
            a ((sprintf Path.Orders.resetVariationsAndEditOrderItemByCategory orderItem.Orderitemid category.Categoryid backUrl))  [] [Text (" "+category.Name)]
    ]

    h2 "Edit"
    renderForm
        { Form = Form.orderItem
          Fieldsets = 
              [ { Legend = "Order item"
                  Fields = 
                      [ 

                        { Label = local.CourseBySelection
                          Html = selectInput (fun f -> <@ f.CourseId @>) idCoursesAndNames (Some ((decimal)orderItem.Courseid))   } 

                        { Label = local.CourseByFreeText
                          Html = formInput (fun f -> <@ f.CourseByName @>)  []  } 

                        { Label = local.ExitGroup
                          Html = selectInput (fun f -> <@ f.GroupOut @>) viableGroupOutIdsForOrderItem (Some ((decimal)orderItem.Groupidentifier))   } 

                        { Label = local.Quantity
                          Html = formInput (fun f -> <@ f.Quantity @>) 
                           [ "Value", (orderItem.Quantity.ToString())] } 

                        { Label = local.Comment
                          Html = formInput (fun f -> <@ f.Comment @>) 
                           [ "Value", (orderItem.Comment) ] } 

                        { Label = local.Price
                          Html = formInput (fun f -> <@ f.Price @>) 
                           [ "Value", formatDec (orderItem.Price) ] } 

                          ] } ]
          SubmitText = local.Update }
    br []
    br []

    div [] [
        a Path.home [] [Text local.MainPage]
    ]
    script ["type", "text/javascript"; "src", "/jquery-3.1.1.min.js" ] []
    script [] [Raw("var pricesForCourses = "+jsPricesForCourses)]
    script ["type","text/javascript"; "src","/script_options_add_order_item.js?kzXq"] [  ]
]



let createCourseCategory msg  = [ 
    h2 local.NewCategory

    div ["id", "register-message"] [
       Text msg
    ]
    renderForm
        { Form = Form.courseCategoryEdit
          Fieldsets = 
              [ { Legend =  local.Category
                  Fields = 
                      [ 
                        { Label = local.Name
                          Html = formInput (fun f -> <@ f.Name @>) [] }

                        { Label = local.Visibility
                          Html = selectInput (fun f -> <@ f.Visibility @>) visibilityType (Some "VISIBLE") } 

                        { Label = local.Abstract
                          Html = selectInput (fun f -> <@ f.Abstract @>) abstractType (Some "CONCRETE") } 

                          ] } 
                          
                          ]
          SubmitText = "Aggiungi categoria" }
    div [] [
        a Path.home [] [Text "Pagina principale"]
    ]
]

let createCourseByCategory message visibleCategories categoryId  = 
    [
    h2 local.CreateNewCourse
    div ["id", "register-message"] [
            Text message
    ]

    renderForm
        { Form = Form.course
          Fieldsets = 
              [ { Legend = local.Course
                  Fields = 
                      [ 
                        { Label = local.Name
                          Html = formInput (fun f -> <@ f.Name @>) [] }
                        { Label = local.Price
                          Html = formInput (fun f -> <@ f.Price @>) [] }
                        { Label = local.Description
                          Html = formInput (fun f -> <@ f.Description @>) [] } 
                        { Label = local.Visibility
                          Html = selectInput (fun f -> <@ f.Visibile @>) visibilityType (Some "VISIBLE") } 
                        { Label = local.Category
                          Html = selectInput (fun f -> <@ f.CategoryId @>) visibleCategories (Some categoryId) } 

                          ] } ]
          SubmitText = local.Add }
    div [] [
        a Path.home [] [Text local.MainPage]
    ]
]

let createCourse visibleCategories   = 
    [
        h2 local.CreateNewCourse
        renderForm
            { Form = Form.course
              Fieldsets = 
                  [ { Legend = local.Course
                      Fields = 
                          [ 
                            { Label = local.Name
                              Html = formInput (fun f -> <@ f.Name @>) [] }
                            { Label = local.Price
                              Html = formInput (fun f -> <@ f.Price @>) [] }
                            { Label = local.Description
                              Html = formInput (fun f -> <@ f.Description @>) [] } 
                            { Label = local.Visibility
                              Html = selectInput (fun f -> <@ f.Visibile @>) visibilityType (Some "VISIBLE") } 
                            { Label = local.Category
                              Html = selectInput (fun f -> <@ f.CategoryId @>) visibleCategories None } 

                              ] } ]
              SubmitText = local.Submit }
        div [] [
            a Path.home [] [Text local.MainPage ]
    ]
]


let createCourseWithPreFilledCategory visibleCategories categoryId  = 
    [
    h2 local.CreateNewCourse
    renderForm
        { Form = Form.course
          Fieldsets = 
              [ { Legend = local.Course
                  Fields = 
                      [ 
                        { Label = local.Name
                          Html = formInput (fun f -> <@ f.Name @>) [] }
                        { Label = local.Price
                          Html = formInput (fun f -> <@ f.Price @>) [] }
                        { Label = local.Description
                          Html = formInput (fun f -> <@ f.Description @>) [] } 
                        { Label = local.Visibility
                          Html = selectInput (fun f -> <@ f.Visibile @>) visibilityType (Some "VISIBLE") } 
                        { Label = local.Category
                          Html = selectInput (fun f -> <@ f.CategoryId @>) visibleCategories (Some categoryId) } 

                          ] } ]
          SubmitText = local.Submit }

    div [] [
        
        a Path.home [] [Text local.MainPage]
    ]
]

let seeCategories (categories: Db.CourseCategories list )  =
    [
    h2 local.ManageCategories;  a Path.Courses.manageAllCategories ["class","buttonX"] [Text local.ManageCategories] 
    a Path.Courses.addCategory ["class","buttonX"] [Text local.AddCategory]
    Text local.StoredCategories;
    ulAttr ["id", "item-list"] [
            for category in categories  ->           
                        tag "innerp" [] [
                          a (sprintf Path.Courses.editCategory category.Categoryid) ["class","buttonX"] [Text (local.EditCategory + category.Name) ] 
                          br []
                        ]
                ]
          ]
          

let seeVisibleCourses category (courses: Db.Coursedetails list) =
    [
    h2 (local.ManageCourses+category);  
    Text local.StoredCourses;  
    ulAttr ["id", "item-list"] [
            for course in courses  ->           
                        tag "innerp" [] [
                          tag "p" [] [a (sprintf Path.Courses.editCourse course.Courseid ) [] [Text (course.Name + " "+course.Coursecategoryname) ] ]
                        ]
            ]
    ]

let seeVisibleCoursesOfACategory (category:Db.CourseCategories option) (courses: Db.Coursedetails list) =
    match category with 
    | Some X  ->
        [
            h2 (local.ManageVisibleCoursesOfCategory+X.Name); 
            tag "p" [] [  a (sprintf Path.Courses.manageAllCoursesOfACategory X.Categoryid) ["class", "buttonX" ] [Text (local.SeeAll+X.Name)]]

            tag "p" [] [ a (sprintf Path.Courses.addCourseByCategory ((int)X.Categoryid)) ["class", "buttonX"] [Text (local.AddNew+X.Name)]]
            br []
            div [] [Text(local.VisibleCoursesOfCategory+X.Name+" "+local.ClickToChange)]
            br []
            ulAttr ["id", "item-list"] [
                for course in courses  ->           
                              tag "p" [] [a (sprintf Path.Courses.editCourse course.Courseid ) [] [Text (course.Name) ]]
                    ]
            a Path.Courses.adminCategories [] [Text local.GoBack]
        ]
    | None -> [ Text("an error occurred, unexisting category")]


let seeAllCourses (category:Db.CourseCategories option)  (courses: Db.Coursedetails list) =
    match category with 
    | Some X -> 
        [
            h2 (local.AllTheItems+X.Name); 
            tag "p" [] [ a (sprintf Path.Courses.manageVisibleCoursesOfACategory X.Categoryid) ["class", "buttonX"] [Text (local.VisibleOfCategory + X.Name)]]
            
            tag "p" [] [ a (sprintf Path.Courses.addCourseByCategory ((int)X.Categoryid)) ["class","buttonX"] [Text (local.AddNewItem + X.Name)]]
            br []
            div [] [Text(local.CoursesOfCategory+X.Name+": " + local.ClickToChange)]
            br []

            ulAttr ["id", "item-list"] [
                for course in courses  ->           
                              tag "p" [] [a (sprintf Path.Courses.editCourse course.Courseid ) [] [Text (course.Name) ]]
                    ]
            a Path.Courses.adminCategories [] [Text local.GoBack]
        ]
    | None -> [ Text("an error occurred, unexisting category")]


let seeAllCoursesPaginated (category:Db.CourseCategories option)  (courses: Db.Coursedetails list) (numberOfPages: int) (pageNumber: int) =
    let nextPageLink (cat:Db.CourseCategories) i = if (i<numberOfPages) then [a ( sprintf Path.Courses.manageAllCoursesOfACategoryPaginated cat.Categoryid (i + 1)) ["class","noredstyle"] [Text (">")]] else []
    let previousPageLink  (cat:Db.CourseCategories) i = if (i>0) then [a ( sprintf Path.Courses.manageAllCoursesOfACategoryPaginated cat.Categoryid (i - 1)) ["class","noredstyle"] [Text ("<")]] else []

    match category with 
    | Some theCategory -> 
        [
            h2 (local.AllVisibleCoursesOfCategory + theCategory.Name); 
            tag "p" [] [ a (sprintf Path.Courses.manageVisibleCoursesOfACategoryPaginated theCategory.Categoryid 0) 
                ["class", "buttonX"] [Text (local.AllVisibleCoursesOfCategory + theCategory.Name)]]
            
            tag "p" [] [ a (sprintf Path.Courses.addCourseByCategory ((int)theCategory.Categoryid)) 
                ["class","buttonX"] [Text (local.AddNewItem+theCategory.Name)]]
            br []
            div [] [Text(local.CoursesOfType+theCategory.Name+": "+local.ClickToChange)]
            br []

            renderForm 
                { Form = Form.searchCourse
                  Fieldsets =
                    [ { Legend = local.SearchByName
                        Fields = 
                            [
                                {
                                    Label = local.Name
                                    Html = formInput (fun f -> <@ f.Name @>) []
                                }
                            ]
                      }
                      ]
                  SubmitText = local.Search 
                }
            br []
            table [
                for course in courses  ->           
                tr [
                    td [
                      a (sprintf Path.Courses.editCourse course.Courseid  ) [] [Text (course.Name) ]
                    ]
                    td [
                      Text((course.Price |> string)  )
                    ]
                ]
            ]

            div ["class","redstyle"] ((previousPageLink theCategory pageNumber) @ (([0 .. numberOfPages] |> (List.map (fun i ->

                if (i = pageNumber) then
                ((a ( sprintf Path.Courses.manageAllCoursesOfACategoryPaginated theCategory.Categoryid i)) ["class","redstyle"] [Text (((i |> string)+" "))])
                else 
                ((a ( sprintf Path.Courses.manageAllCoursesOfACategoryPaginated theCategory.Categoryid i)) ["class","noredstyle"] [Text (((i |> string)))])

                )) 
            ) @ (nextPageLink theCategory pageNumber)))

            div [] [a Path.Courses.adminCategories [] [Text local.GoBack]]
        ]
    | None -> [ Text("an error occurred, unexisting category")]



let seeVisibleCoursesPaginated (category:Db.CourseCategories option)  (courses: Db.Coursedetails list) (numberOfPages: int) (pageNumber: int) =
    let nextPageLink (cat:Db.CourseCategories) i = if (i<numberOfPages) then [a ( sprintf Path.Courses.manageVisibleCoursesOfACategoryPaginated cat.Categoryid (i + 1)) ["class","noredstyle"] [Text (">")]] else []
    let previousPageLink  (cat:Db.CourseCategories) i = if (i>0) then [a ( sprintf Path.Courses.manageVisibleCoursesOfACategoryPaginated cat.Categoryid (i - 1)) ["class","noredstyle"] [Text ("<")]] else []
    match category with 
    | Some theCategory -> 
        [
            h2 (local.AllVisibleCoursesOfCategory + theCategory.Name); 
            tag "p" [] [ a (sprintf Path.Courses.manageAllCoursesOfACategoryPaginated theCategory.Categoryid 0) 
                ["class", "buttonX"] [Text (local.ClickToSeeAllVisibleCoursesOfCategory+theCategory.Name)]]
            
            tag "p" [] [ a (sprintf Path.Courses.addCourseByCategory ((int)theCategory.Categoryid)) 
                ["class","buttonX"] [Text (local.AddNewItem + theCategory.Name)]]
            br []
            div [] [Text(local.CoursesOfCategory + theCategory.Name+":"+ local.ClickToChange)]
            br []

            table [
                    for course in courses  ->           
                    tr [
                        td [
                          a (sprintf Path.Courses.editCourse course.Courseid  ) [] [Text (course.Name) ]
                        ]
                        td [
                          Text((course.Price |> string)  )
                        ]
                    ]
            ]

            div ["class","redstyle"] ((previousPageLink theCategory pageNumber) @ (([0 .. numberOfPages] |> (List.map (fun i ->

                if (i = pageNumber) then
                ((a ( sprintf Path.Courses.manageVisibleCoursesOfACategoryPaginated theCategory.Categoryid i)) ["class","redstyle"] [Text (((i |> string)+" "))])
                else 
                ((a ( sprintf Path.Courses.manageVisibleCoursesOfACategoryPaginated theCategory.Categoryid i)) ["class","noredstyle"] [Text (((i |> string)))])
                )) 
            ) @ (nextPageLink theCategory pageNumber)))

            div [] [a Path.Courses.adminCategories [] [Text local.GoBack]]
        ]
    | None -> [ Text("an error occurred, unexisting category")]



let fillIngredient message (ingredient:Db.Ingredient) (allIngredientCategories: Db.IngredientCategory list) (backUrl:string) =

    [
        tag "h1" [] [Text (local.Ingredient + ": " + ingredient.Name)]

        div ["id", "register-message"] [
            Text message
        ]

        h2 (local.AddQuantityIn + ingredient.Unitmeasure)
        renderForm
            { Form = Form.ingredientLoad
              Fieldsets = 
                  [ { Legend = local.Ingredient
                      Fields = 
                          [ 
                            { Label = local.Quantity + ingredient.Unitmeasure
                              Html = formInput (fun f -> <@ f.Quantity @>) ["value","0"] }
                            { Label = local.Comment
                              Html = formInput (fun f -> <@ f.Comment @>) ["value",""] } 

                              ] } ]
              SubmitText = local.Submit  }
        br []
        div [] [
            a (backUrl) [] [Text(local.GoBack)]
        ]
    ]


let editIngredient message (ingredient:Db.Ingredient) (allIngredientCategories: Db.IngredientCategory list) (backUrl:string) =
    let allIngredientCategoriesIdName = List.map (fun (x:Db.IngredientCategory) -> ((decimal)x.Ingredientcategoryid, x.Name)) allIngredientCategories

    [
        tag "h1" [] [Text (local.Ingredient + ingredient.Name)]

        div ["id", "register-message"] [
            Text message
        ]

        h2 local.ModifyIngredient
        renderForm
            { Form = Form.ingredientEdit
              Fieldsets = 
                  [ { Legend = local.Ingredient
                      Fields = 
                          [ 
                            { Label = local.Name
                              Html = formInput (fun f -> <@ f.Name @>) ["value",ingredient.Name] }
                            { Label = local.Description
                              Html = formInput (fun f -> <@ f.Comment @>) ["value",ingredient.Description] } 
                            { Label = local.Category
                              Html = selectInput (fun f -> <@ f.Category @>) allIngredientCategoriesIdName (Some ((decimal)ingredient.Ingredientcategoryid)) } 
                            { Label = local.Allergen
                              Html = selectInput (fun f -> <@ f.Allergene @>) yesOrNo (if (ingredient.Allergen) then (Some "YES") else (Some "NO")) }
                            { Label = local.UpdateAvailability
                              Html = selectInput (fun f -> <@ f.UpdateAvailabilityFlag @>) yesOrNo (if (ingredient.Updateavailabilityflag) then (Some "YES") else (Some "NO")) }
                            { Label = local.CheckAvailability
                              Html = selectInput (fun f -> <@ f.CheckAvailabilityFlag @>) yesOrNo (if (ingredient.Checkavailabilityflag) then (Some "YES") else (Some "NO")) }
                            { Label = local.Visibility
                              Html = selectInput (fun f -> <@ f.Visibility @>) visibilityType (if (ingredient.Visibility) then (Some "VISIBLE") else (Some "INVISIBLE")) } 

                            { Label = local.MeasuringSystem
                              Html = selectInput (fun f -> <@ f.UnitOfMeasure @>) unit_of_measures_drop_box (Some ingredient.Unitmeasure) } 

                              ] } ]
              SubmitText = local.Update }
        br []
        div [] [
            a (backUrl) [] [Text(local.GoBack)]
        ]
    ]



let ingredientsOfACategory message (category:Db.IngredientCategory) (allIngredientOfCategory: (Db.Ingredient) list)  =
    [
        tag "h1" [] [Text (local.Category+category.Name+ " ("+local.All+")")]

        div ["id", "register-message"] [
            Text message
        ]

        h2 local.NewIngredient
        renderForm
            { Form = Form.ingredient
              Fieldsets = 
                  [ { Legend = local.Ingredient
                      Fields = 
                          [ 
                            { Label = local.Name
                              Html = formInput (fun f -> <@ f.Name @>) [] }
                            { Label = local.Description
                              Html = formInput (fun f -> <@ f.Comment @>) [] } 
                            { Label = local.Allergen
                              Html = selectInput (fun f -> <@ f.Allergene @>) yesOrNo (Some "NO") } 
                            { Label = local.Visibility
                              Html = selectInput (fun f -> <@ f.Visibility @>) visibilityType (Some "VISIBLE") } 
                            { Label = local.UpdateAvailability
                              Html = selectInput (fun f -> <@ f.UpdateAvailabilityFlag @>) yesOrNo (Some "NO") } 
                            { Label = local.AvailablQuantity
                              Html = formInput (fun f -> <@ f.AvailableQuantity @>) ["Value", "0"]  } 
                            { Label = local.MeasuringSystem
                              Html = selectInput (fun f -> <@ f.UnitOfMeasure @>) unit_of_measures_drop_box (None)  } 
                            { Label = local.CheckAvailability
                              Html = selectInput (fun f -> <@ f.CheckAvailabilityFlag @>) yesOrNo (Some "NO") } 
                              ] } ]
              SubmitText = local.Submit }

        br []
        div [] [
            (a (sprintf  Path.Admin.editIngredientCategoryPaginated category.Ingredientcategoryid 0))  [] [Text(local.GoBack)]
        ]

    ]


let visibleIngreientCategoriesAdministrationPage (visibleIngredientCategories:Db.IngredientCategory list) backUrl =
    [
        tag "h1" [] [Text local.VisibleCategoriesOfIngredients]
        tag "p" [] [ (a (Path.Admin.allIngredientCategories) ["class","buttonX"] [Text(local.SeeAll)])]
        renderForm
            { Form = Form.ingredientCategory
              Fieldsets = 
                  [ { Legend = local.NewCategory 
                      Fields = 
                          [ 
                            { Label = local.Name 
                              Html = formInput (fun f -> <@ f.Name @>) [] }
                            { Label = local.Description 
                              Html = formInput (fun f -> <@ f.Comment @>) [] } 
                            { Label = local.Visibility 
                              Html = selectInput (fun f -> <@ f.Visibility @>) visibilityType (Some "VISIBLE") } 
                              ] } ]
              SubmitText = local.CreateNewCategory }
        br []
        br []
        tag "h2" [] [Text local.VisibleExistingCategories] // "categorie visibili esistenti:"
        ulAttr ["id","item-list"] [
            for ingredientCategory in visibleIngredientCategories  -> 
            let visibility = match ingredientCategory.Visibility with | true -> "VISIBILE" | _ -> "NASCOSTO"
            let viewClass = match ingredientCategory.Visibility with | true -> "buttonEnabled" | _ -> "buttonY"

            tag "p" [] [
                  a (sprintf Path.Admin.editIngredientCategory ingredientCategory.Ingredientcategoryid) ["class",viewClass] 
                    [Text (ingredientCategory.Name + " "+visibility  )]

                  a ((sprintf Path.Admin.switchVisibilityOfIngredientCategory
                    ingredientCategory.Ingredientcategoryid (WebUtility.UrlEncode Path.Admin.visibleIngredientCategories) ))  ["class","buttonX"] [Text(local.ChangeVisibility)]
            ]
        ]
        tag "p" [] [
                  br[]
                  br[]
                  a ((Path.home))  [] [Text(local.GoBack)]
        ]
    ]

let about =
    [
        Text("Orders System. Copyright: Antonio Lucca/Luckysoft. Tonyx1@gmail.com. This software is released under the term of the General Public License")
    ]

let adminPrinters (printers: Db.Printer list)=
    [
        h2 (local.ManagePrinters)
        br[]
        tag "p" [] [ (a (Path.Admin.recognizePrinters) ["class","buttonX"] [Text(local.DetectPrinters)])]

        ulAttr ["id","item-list"] [
            for printer in printers ->
            tag "p" [] [
                 (a (sprintf Path.Admin.managePrinter printer.Printerid -1) ["class","buttonX"] [Text(printer.Name)])
            ]
        ]
    ]

let ingredientCatgoriesAdministrationPage  (allIngredientCategories:Db.IngredientCategory list)   =
    [
        tag "h1" [] [Text local.AllCategoriesOfIngredients]
        tag "p" [] [ (a (Path.Admin.visibleIngredientCategories) ["class","buttonX"] [Text(local.SeeOnlyVisibles)])]
        renderForm
            { Form = Form.ingredientCategory
              Fieldsets = 
                  [ { Legend = local.NewCategory
                      Fields = 
                          [ 
                            { Label = local.Name
                              Html = formInput (fun f -> <@ f.Name @>) [] }
                            { Label = local.Description 
                              Html = formInput (fun f -> <@ f.Comment @>) [] } 
                            { Label = local.Visibility 
                              Html = selectInput (fun f -> <@ f.Visibility @>) visibilityType (Some "VISIBLE") } 
                              ] } ]
              SubmitText = local.CreateNewCategory }
        br []
        br []

        tag "h2" [] [Text local.ExistingCategories ]
        ulAttr ["id","item-list"] [
            for ingredientCategory in allIngredientCategories  -> 
            let visibility = match ingredientCategory.Visibility with | true -> "VISIBILE" | _ -> "NASCOSTO"
            let buttonClass = match ingredientCategory.Visibility with | true -> "buttonEnabled" | _ ->  "buttonY"

            tag "p" [] [
                  a (sprintf Path.Admin.editIngredientCategoryPaginated ingredientCategory.Ingredientcategoryid 0) ["class",buttonClass] 
                    [Text (ingredientCategory.Name + " "+visibility  )]
                  a ((sprintf Path.Admin.switchVisibilityOfIngredientCategory
                    ingredientCategory.Ingredientcategoryid (WebUtility.UrlEncode Path.Admin.allIngredientCategories)))  ["class","buttonX"] [Text("modifica visibilità")]
            ]

        ]
        tag "p" [] [
                  br[]
                  br[]
                  a ((Path.home))  [] [Text(local.GoBack)]
        ]
    ]

let addQrUser = [
    renderForm
        { Form = Form.qrUser
          Fieldsets = 
              [ { Legend = local.TemporaryUser
                  Fields = 
                      [ 
                        { Label = local.NameOrTable
                          Html = formInput (fun f -> <@ f.TableName @>) [] }
                          ] } ]
          SubmitText = local.Submit }
]


let recycleQrUser  = [
    renderForm
        { Form = Form.qrUser
          Fieldsets = 
              [ { Legend = local.TemporaryUser
                  Fields = 
                      [ 
                        { Label = local.NameOrTable
                          Html = formInput (fun f -> <@ f.TableName @>) [] }
                          ] } ]
          SubmitText = local.RegenerateTempUser }
]


let temporaryUsersAdministrationPage  (users: Db.UsersView list) =
     [
         tag "h1" [] [Text local.ManageTemporaryUsers];

         a Path.Extension.addQruser ["class","buttonX"] [Text local.CreateTemporaryUsers] 

         ulAttr ["id ","item-list"] [
             for user in users  -> 
                let enabledview = match user.Enabled with
                | true -> "enabled"
                | false -> "disabled"

                let isExpired = user.Creationtime.AddMinutes(Globals.EXPIRATION_TIME_TEMPORARY_USERS).CompareTo(System.DateTime.Now)<0
                let regenLink = 
                    if (isExpired) then (a (sprintf Path.Extension.regenTempUser user.Userid ) [] [Text local.UserExpired])  else em ""
                tag "p" [] [
                    a (sprintf Path.Admin.editTemporaryUser user.Userid) [] [Text (user.Username+" "+user.Rolename+" "+enabledview)]
                    regenLink
                    br []
                ]
         ]

     ]


let userAdministrationPage  (users: Db.UsersView list) =
     [
         tag "h1" [] [Text local.ManageUsers ];
         a Path.Admin.addUser ["class","buttonX"] [Text local.AddUser ] 

         br []
         br []
         table [
             tr [
             td [Text(local.UserName)]
             td [Text(local.Role)]
             td [Text(local.EnableState)]
             td [Text(local.ViewStates)]
             ]
         ]
         
         table [
             for user in users  -> 
                let enabledview = match user.Enabled with
                | true -> "enabled"
                | false -> "disabled"

                tr [
                    
                   td [a (sprintf Path.Admin.editUser user.Userid) [] [Text (user.Username)]]
                   td [Text (user.Rolename)]
                   td [Text (enabledview)]
                   td [a (sprintf Path.Admin.actionableStatesForSpecificOrderOwner user.Userid) ["class","buttonX"] [Text(local.LinkStates)]]
                ]
         ]
     ]
     
let objectDeletionPage = 
 [
        tag "h1" [] [Text local.DeletionPage] 
        tag "h2" [] [Text local.WarningDeletion]
        tag "p" [] [
            a ( Path.Admin.deleteUsers ) ["class","buttonX"] [Text local.DeleteUsers ]
        ]
        tag "p" [] [
            a ( Path.Admin.deleteTemporaryUsers ) ["class","buttonX"] [Text local.DeleteTemporaryUsers ]
        ]
        tag "p" [] [
            a ( Path.Admin.deleteCourses ) ["class","buttonX"] [Text local.DeleteCourses]
        ]
        tag "p" [] [
            a ( Path.Admin.deleteCourseCategories ) ["class","buttonX"] [Text local.DeleteCourseCategories]
        ]
        tag "p" [] [
            a ( Path.Admin.deleteUserRoles ) ["class","buttonX"] [Text local.DeleteRoles]
        ]
        tag "p" [] [
            a ( Path.Admin.deleteIngredients ) ["class","buttonX"] [Text local.DeleteIngredients ]
        ]
        tag "p" [] [
            a ( Path.Admin.deleteIngredientCategories ) ["class","buttonX"] [Text local.DeleteIngredientCategories]
        ]
 ]

let rolesDeletionPage (roles: Db.Role list ) =
    [
        tag "h1" [] [Text local.DeleteRoles ]
        ulAttr ["id ","item-list"] [
            for role in roles ->
                tag "p" [] [a (sprintf Path.Admin.deleteRole role.Roleid) ["class","buttonX"] [Text (local.Delete+role.Rolename)]] 
        ]
    ]

let ingredientsDeletionPage (ingredients: Db.Ingredient list ) =
    [
        tag "h1" [] [Text local.DeleteIngredients]
        ulAttr ["id ","item-list"] [
            for ingredient in ingredients ->
                tag "p" [] [a (sprintf Path.Admin.deleteIngredient ingredient.Ingredientid) ["class","buttonX"] [Text (local.Delete+ingredient.Name)]] 
        ]
    ]

let userDeletionPage (users: Db.UsersView list) backUrl =
    [
        tag "h1" [] [Text local.DeleteUsers]
        ulAttr ["id ","item-list"] [
            for user in users ->
                tag "p" [] [a ((sprintf Path.Admin.deleteUser user.Userid)+"?backUrl="+backUrl) ["class","buttonX"] [Text (local.Delete+user.Username)]] 
        ]
    ]
let ingredientCategoriesDeletionPage (ingredientCategories: Db.IngredientCategory list) =
    [
        tag "h1" [] [Text local.DeleteIngredientCategories]
        ulAttr ["id ","item-list"] [
            for ingCategory in ingredientCategories ->
                tag "p" [] [a (sprintf Path.Admin.deleteIngredientCategory ingCategory.Ingredientcategoryid) ["class","buttonX"] [Text (local.Delete+ingCategory.Name)]] 
        ]
    ]


let courseCategoriesDeletionPage (courseCategories: Db.CourseCategories list) =
    [
        tag "h1" [] [Text local.DeleteCourseCategories]
        ulAttr ["id ","item-list"] [
            for courseCategory in courseCategories ->
                tag "p" [] [a (sprintf Path.Admin.deleteCourseCategory courseCategory.Categoryid) ["class","buttonX"] [Text (local.Delete+courseCategory.Name)]] 
        ]
    ]



let ingredientsDeletionPageBySelect (ingreients: Db.Ingredient list) =
    let ingredientsMap = ingreients |> List.map (fun (x:Db.Ingredient) -> ((decimal)x.Ingredientid, x.Name))
    [
    tag "h1" [] [Text local.DeleteIngredients]

    renderForm

       { Form = Form.ingredientDeletion
         Fieldsets = 
             [ { Legend = local.Course
                 Fields = 
                     [ 
                       { Label = local.NameBySelection
                         Html =  selectInput 
                                   (fun f -> <@ f.IngredientId  @>) 
                                    ingredientsMap (if (List.isEmpty ingredientsMap) then None else  (Some (fst(List.head ingredientsMap))))
                                    
                                    }
                       { Label = local.NameByFreeText
                         Html =  formInput 
                                   (fun f -> <@ f.IngredientName  @>) []}
                                   ] } 
                                   ]
         SubmitText = local.Remove  }

    tag "p" [] [a (Path.Admin.deleteObjects) []  [Text(local.GoBack)]]

    script ["type","text/javascript"; "src","/autocompleteDelIngredients.js"] []

    ]



let coursesDeletionPage (courses: Db.Course list) =
    let coursesMap = courses |> List.map (fun (x:Db.Course) -> ((decimal)x.Courseid, x.Name))
    [
    tag "h1" [] [Text local.DeleteCourses ]

    renderForm

       { Form = Form.courseDeletion
         Fieldsets = 
             [ { Legend = local.Course
                 Fields = 
                     [ 
                       { Label = local.NameBySelection
                         Html =  selectInput 
                                   (fun f -> <@ f.CourseId  @>) 
                                    coursesMap (if (List.isEmpty coursesMap) then None else  (Some (fst(List.head coursesMap))))
                                    
                                    }
                       { Label = local.NameByFreeText
                         Html =  formInput 
                                   (fun f -> <@ f.CourseName  @>) []}
                                   ] } 
                                   ]
         SubmitText = local.Remove  }
    br []

    tag "p" [] [a (Path.Admin.deleteObjects) []  [Text(local.GoBack)]]

    script ["type","text/javascript"; "src","/autocompleteDelCourses.js"] []

    ]


let createRole (user:UserLoggedOnSession) =
   match user.Role with
   | "admin"  ->
      [  renderForm
        { Form = Form.role
          Fieldsets = 
              [ { Legend = local.Role
                  Fields = 
                      [ 
                        { Label = local.RoleName
                          Html = formInput (fun f -> <@ f.Name @>) [ ] } 
                        { Label = local.Comment
                          Html = formInput (fun f -> <@ f.Comment @>) [ ] } 
                          ] } ]
          SubmitText = local.CreateRole  } 

      ]
    | _ -> failwith "user is not enabled"



let rolesAdministrationPage  (roles: Db.Role list) (allRolesWithObservers:Db.ObserverRoleStatusCategory list)  (allRolesWithEnablers:Db.EnablerRoleStatusCategory list)  =


    let roleEnablerObserverCategoriesByCheckBoxes = tag "p" [] [a Path.Admin.roleEnablerObserverCategoriesByCheckBoxes   ["class","buttonX"] [Text local.AccessRights]]

    let defaultStateEnabler = tag "p" [] [a Path.Admin.defaultActionableStatesForOrderOwner   ["class","buttonX"] [Text local.DefaultStatesForWaiter]]
    let tempUserDefaultStateEnabler = tag "p" [] [a Path.Admin.tempUserDefaultActionableStates   ["class","buttonX"] [Text local.DefaultStatesForTemporaryUsers]]

    [
         tag "h1" [] [Text local.ManageRoles ];
         tag "p" [] [a Path.Admin.addRole ["class","buttonX"] [Text local.AddRole] ]

         roleEnablerObserverCategoriesByCheckBoxes

         defaultStateEnabler
         tempUserDefaultStateEnabler

         tag "h2" [] [Text local.ListOfExistingRoles]

         ulAttr ["id ","item-list"] [
             for role in  roles -> 
                tag "p" [] [
                    strong (role.Rolename)
                    br []
                ]
         ]
    ]

let coursesAdministrationPage  (categories: Db.CourseCategories list) =
     [
         tag "h1" [] [Text local.CourseCategoriesManagement ] 
         br []

         tag "p" [] [a Path.Courses.addCategory ["class","buttonX"] [Text (local.AddCategory)]]
         table
            [
             for category in categories  -> 
                let (enabledview,buttonVisibility) = match category.Visibile with
                | true -> ("visibile","buttonEnabled")
                | false -> ("nascosto","buttonY")

                tr [
                tag "p" [] 
                   [  
                      td [a (sprintf Path.Courses.manageAllCoursesOfACategoryPaginated category.Categoryid 0) ["class",buttonVisibility]  [Text(category.Name)]]
                      td (if (category.Abstract) then [(Text(local.Abstract))] else [(Text(""))])
                      td [a (sprintf Path.Courses.switchCourseCategoryVisibility category.Categoryid) ["class","buttonX"]  [Text(local.Visibility)]]
                      td [a (sprintf Path.Courses.editCategory category.Categoryid) ["class","buttonX"]  [Text(local.Name )]]
                ]
                ]
         ]
     ]

let ingredientAdminLInk user = 
        match (user.Role,user.CanManageAllCourses) with
        | ("admin",_) | (_,true) -> tag "p" [] [a Path.Admin.allIngredientCategories ["class","buttonX"] [Text local.ManageIngredients]]
        | _ -> em ""

let printersAdminLink user = 
        match user.Role with
        | "admin"   -> tag "p" [] [a Path.Admin.printers ["class","buttonX"] [Text local.ManagePrinters]]
        | _ -> em ""

let info user = 
        match user.Role with
        | "admin"   -> tag "p" [] [a Path.Admin.info ["class","buttonX"] [Text local.Info]]
        | _ -> em ""

let allOrdersLink user = 
    match user.Role with 
    | "admin"  -> tag "p" [] [a Path.Orders.allOrders ["class","buttonX"] [Text local.AllTheOrders]]
    | _ -> em ""

let categoriesLink user = 
    match (user.Role,user.CanManageAllCourses) with 
    | ("admin",_) | (_,true)  -> tag "p" [] [a Path.Courses.adminCategories  ["class","buttonX"] [Text local.CourseCategories ]]
    | _ -> em ""

let rolesLink user =
    match user.Role with
    | "admin" -> tag "p" [] [a Path.Admin.roles   ["class","buttonX"] [Text local.ManageRoles]]
    | _ -> em ""

let allUsersLink user = 
    match user.Role with
    | "admin" ->  tag "p" [] [a (Path.Admin.allUsers) ["class","buttonX"] [Text local.ManageOrdinaryUsers]]
    | _ -> em  ""

let temporaryUsersLink user =
    match user.Role with
    | "admin" ->  tag "p" [] [a (Path.Admin.temporaryUsers) ["class","buttonX"] [Text local.ManageTemporaryUsers]]
    | _ -> em  ""

let seeDoneOrdersLink (dbUser:Db.User) =
    match  dbUser.Canmanageallorders with
    | true -> tag "p" [] [a (Path.Orders.seeDoneOrders) ["class","buttonX"] [Text local.ManagePayments]]
    | _ -> em ""

let optimizeVoidedLink (dbUser:Db.User) =
    match  dbUser.Canmanageallorders with
    | true -> tag "p" [] [a (Path.Admin.optimizeVoided) ["class","buttonX"] [Text local.Optimization]]
    | _ -> em ""

let deleteObjectsPage user =
    match user.Role = "admin"  with
    | true -> tag "p" [] [a (Path.Admin.deleteObjects) ["class","buttonX"] [Text local.Deletions]]
    | _ -> em ""

let changePasswordButton =
     tag "p" [] [a (Path.Account.changePassword) ["class","buttonX"] [Text local.ChangePassword]]

let myOrdersButton =
    tag "p" [] [a Path.Orders.myOrders  ["class","buttonX"] [Text "mie  comande (totali)"]]

let myOrdersButtonSingles =
    tag "p" [] [a Path.Orders.myOrdersSingles  ["class","buttonX"] [Text local.Orders]]

let logOffButton =
    tag "p" [] [a Path.Account.logoff ["class","buttonX"] [Text local.Logoff]] 

let empty (user:UserLoggedOnSession) (dbUser: Db.User) =
    [
        Text("it works")
    ]
let controlPanel (user:UserLoggedOnSession) (dbUser: Db.User)=
    let orderItemsProgressLink = tag "p" [] [a (Path.Orders.orderItemsProgress) ["class","buttonX"] [Text local.OrderItemStates]]
    [
        br []
        br []
        strong (local.ConnectedUser+": "+user.Username+ ".  ")
        strong (local.UserRole+user.Role)
        br []
        br []
        br []
        info user
        printersAdminLink user
        ingredientAdminLInk user
        categoriesLink user
        rolesLink user
        allUsersLink user
        temporaryUsersLink user
        orderItemsProgressLink
        seeDoneOrdersLink  dbUser
        changePasswordButton
        myOrdersButtonSingles
        logOffButton
        deleteObjectsPage user
    ]

let coursesAndCategoriesManagement  (categories:Db.CourseCategories list) =
    [
        tag "p" [] [a Path.Courses.adminCategories ["class","buttonX"] [Text local.CourseCategoriesManagement ]] 
        tag "p" [] [a Path.Courses.manageAllCourses ["class","buttonX"] [Text local.AllCourses]]
        tag "innerp" [] [for category in categories -> tag "p" [] [a (sprintf Path.Courses.manageVisibleCoursesOfACategory category.Categoryid) ["class","buttonX"] [Text (local.Manage + category.Name + " ")] ]]
    ]

let viewableOrderItems (orderItems: OrderItemDetails list) (mapOfLinkedStates: Map<int,State>) (mapOfVariations: Map<int,Db.VariationDetail list>) (strIngredientsOfCourses: Map<int,string>) (variationsStringDescriptions: Map<int,string>) = 
    [
    h2  "order item in progress"  
    tag "p" []  [
       table  [
         for orderItem in orderItems  -> 
         let variationsDesc = "var: "+variationsStringDescriptions.[orderItem.Orderitemid]
         let receiptDesc = if (strIngredientsOfCourses.[orderItem.Orderitemid] <> "") then strIngredientsOfCourses.[orderItem.Orderitemid] else local.Missing
         p [] 
            [
               tr 
                [ td 
                   [
                       br []

                       Text(local.Table+orderItem.Table+": "+local.Quantity+": "+   orderItem.Quantity.ToString()+" "+orderItem.Name+" "+orderItem.Comment+" " + 
                         orderItem.Statusname+ " resp. " + orderItem.Username + " cli.: " + orderItem.Person 
                         + local.Group  + (sprintf "%d" orderItem.Groupidentifier) + local.Receipt+ receiptDesc  )

                       br []
                       Text(variationsDesc)

                       br []
                       a (sprintf Path.Orders.moveOrderItemToTheNextStateAndGoOrdersProgress orderItem.Orderitemid) ["class","buttonX"] 
                        [Text ("->: "+  mapOfLinkedStates.[orderItem.Stateid].Statusname)]

                       a (sprintf Path.Orders.rejectOrderItem  orderItem.Orderitemid ) ["class","buttonX"] 
                        [Text local.Reject]
                       br []
                   ]
                ]
            ]
         ]
    ]
    script ["type", "text/javascript"; "src", "/autorefresh.js" ] []
] 

let rejectOrderItem orerItem =
    [
        h2 (local.RejectOrderItem)
        renderForm 
         {
            Form  = Form.orderItemRejection
            Fieldsets =
                [
                    {
                        Legend = local.RejectOrderItem
                        Fields =
                            [
                                {
                                    Label =  local.Motivation
                                    Html = formInput (fun f -> <@ f.Motivation @>) []
                                }
                            ]
                    }
                ]
            SubmitText = local.Submit
        }

    ]

let specificActionableStatesForWaiter (user: Db.User) (states: Db.State list) (specificPerUserActionableStates: Db.WaiterActionableState list ) =
    let stateActionEnabled stateId =
        let statesEnabled = List.map (fun (x:Db.WaiterActionableState) -> x.Stateid) specificPerUserActionableStates
        List.contains stateId statesEnabled
    [   
        tag "h1" [] [Text(local.ActionableStatesForUser +  user.Username)]
        br []
        br []
        br []
        
        ulAttr ["id", "item-list"] [
        for state in states ->
            if (stateActionEnabled state.Stateid) then
                tag "p" [] [ a (sprintf Path.Admin.unSetStateAsActionableForSpecificWaiter user.Userid state.Stateid) 
                    ["class","buttonEnabled"] [Text (state.Statusname+" "+local.Disable)]]
            else 
                tag "p" [] [ a (sprintf Path.Admin.setStateAsActionableForSpecificWaiter user.Userid state.Stateid) 
                    ["class","buttonY"] [Text (state.Statusname+" "+local.Enable)]]
    ]]

let defaultActionableStatesForWaiter (states: Db.State list) (defaultActionableStats: Db.DefaultActionableState list ) =

    let stateActionEnabled stateId =
        let statesEnabled = List.map (fun (x:Db.DefaultActionableState) -> x.Stateid) defaultActionableStats
        List.contains stateId statesEnabled

    [   
        tag "h1" [] [Text(local.ActionableStatesForWaiters) ]
        ulAttr ["id", "item-list"] [
        for state in states ->
            if (stateActionEnabled state.Stateid) then
            
                tag "p" [] [ a (sprintf Path.Admin.unSetStateAsActionableForWaiterByDefault state.Stateid) 
                    ["class","buttonEnabled"] [Text (state.Statusname+local.Disable)]]
            else 
                tag "p" [] [ a (sprintf Path.Admin.setStateAsActionableForWaiterByDefault state.Stateid) 
                    ["class","buttonY"] [Text (state.Statusname+local.Enable)]]
    ]]


let defaultActionableStatesForTempUser (states: Db.State list) (defaultActionableStats: Db.TempUserDefaultActionableStates list ) =

    let stateActionEnabled stateId =
        let statesEnabled = List.map (fun (x:Db.TempUserDefaultActionableStates) -> x.Stateid) defaultActionableStats
        List.contains stateId statesEnabled
    [   
        tag "h1" [] [Text(local.ActionableStatesForTemporaryUsers) ]
        br []
        ulAttr ["id", "item-list"] [
        for state in states ->
            if (stateActionEnabled state.Stateid) then
                tag "p" [] [ a (sprintf Path.Admin.unSetStateAsActionableForTempUserByDefault state.Stateid) 
                    ["class","buttonEnabled"] [Text (state.Statusname+local.Disable)]]
            else 
                tag "p" [] [ a (sprintf Path.Admin.setStateAsActionableForTempUserByDefault state.Stateid) 
                    ["class","buttonY"] [Text (state.Statusname+local.Enable)]]
    ]]

let seeSingleOrder (order: Db.Order) (orderItemsdetailsOfOrder:Db.OrderItemDetails list) =
    let discountMessage = match (order.Adjustispercentage,order.Adjustisplain) with
        | (true,_) -> 
             local.Variation+
             (if (order.Percentagevariataion > (decimal) 0) then "+" else "" ) +
             (sprintf "%.0f"  order.Percentagevariataion)  +  " % (" +
             (if (order.Percentagevariataion > (decimal) 0) then "+" else "" ) +
             (sprintf "%.2f"  (((order.Percentagevariataion)/(decimal)100)*(order.Total))) + ") = " +
             (sprintf "%.2f" order.Adjustedtotal)
        | (_,true) ->   local.Variation + 
            (order.Plaintotalvariation.ToString()) + " = " + order.Adjustedtotal.ToString()
        | _ -> ""

    [
         ulAttr ["id","item-list"] [
            tag "innerp" [] [
                tag "a" ["name","order"+((order.Orderid) |> string)] []
                br[]
                Text(local.Total+order.Total.ToString())
                Text(discountMessage)

                br []
                table [
                    for orderItem in orderItemsdetailsOfOrder ->
                     tr
                       [
                       td [ 
                             a (sprintf Path.Orders.editDoneOrderitem orderItem.Orderitemid orderItem.Categoryid ) ["",""] 
                              [Text(orderItem.Quantity.ToString()+" "+orderItem.Name+" "+orderItem.Comment+" "+ (string)orderItem.Price  
                                + (if (orderItem.Quantity>1) then " * " + (string)(orderItem.Quantity)  +    " = "+
                                (string)((decimal)orderItem.Quantity*orderItem.Price) else "")
                              )
                              ]
                          ]
                       ]
                ]
            ]
        ]
    ]

let seeOrder (order:Db.Order) (orderItemsdetailsOfOrder:Db.OrderItemDetails list) backUrl = 

    let discountMessage = match (order.Adjustispercentage,order.Adjustisplain) with
        | (true,_) -> 
             local.Variation +
             (if (order.Percentagevariataion > (decimal) 0) then "+" else "" ) +
             (sprintf "%.0f"  order.Percentagevariataion)  +  " % (" +
             (if (order.Percentagevariataion > (decimal) 0) then "+" else "" ) +
             (sprintf "%.2f"  (((order.Percentagevariataion)/(decimal)100)*(order.Total))) + ") = " +
             (sprintf "%.2f" order.Adjustedtotal)
        | (_,true) ->    local.Variation +
            (order.Plaintotalvariation.ToString()) + " = " + order.Adjustedtotal.ToString()
        | _ -> ""

    [
        renderForm
            { Form = Form.priceAdjustment
              Fieldsets = 
                  [ { Legend = local.VariationOverPriceDiscount
                      Fields = 
                          [ 
                            { Label = local.Value
                              Html = formInput (fun f -> <@  f.Value @>) [ "value", 
                               (if (order.Adjustispercentage) then (sprintf "%.0f" order.Percentagevariataion) else 
                               (sprintf "%.2f" order.Plaintotalvariation)) ] } 
                            { Label = local.Percentage
                              Html = selectInput (fun f -> <@  f.PercentOrValue @>) percentageOrValue 
                                 ( 
                                     match (order.Adjustispercentage,order.Adjustisplain) with 
                                     | (true,_) -> Some "PERCENTUALE"
                                     | (_,true) -> Some "VALORE"
                                     | _ -> None
                                     
                                     ) 
                                     } 
                              ] } ]
              SubmitText = local.Submit } 

        ulAttr ["id","item-list"] [
            tag "innerp" [] [
                tag "a" ["name","order"+((order.Orderid) |> string)] []
                br[]
                Text(local.Total+order.Total.ToString())
                Text(discountMessage)

                br []
                table [
                    for orderItem in orderItemsdetailsOfOrder ->
                     tr
                       [
                       td [ 
                             a (sprintf Path.Orders.editDoneOrderitem orderItem.Orderitemid orderItem.Categoryid) ["",""] 
                              [Text(orderItem.Quantity.ToString()+" "+orderItem.Name+" "+orderItem.Comment+" "+ (string)orderItem.Price  
                                + (if (orderItem.Quantity>1) then " * " + (string)(orderItem.Quantity)  +    " = "+
                                (string)((decimal)orderItem.Quantity*orderItem.Price) else "")
                              )
                              ]
                             a (sprintf Path.Orders.removeOrderItemThenGoBackToUrl orderItem.Orderitemid (WebUtility.UrlEncode backUrl)) ["class","buttonX"] 
                              [Text(local.Remove)
                              ]

                          ]
                       ]
                ]
                br []
                br []
                tag "p" [] [ a (sprintf Path.Orders.resetDiscount order.Orderid) ["class","buttonX"] [Text local.VoidDiscounts]]
                tag "p" [] [ a Path.Orders.seeDoneOrders ["class","buttonX"] [Text local.End ]]
            ]
        ]
    ]

let viewSingleOrder (order: Db.Orderdetail) (orderItems: Db.OrderItemDetails list) (mapOfStates: Map<int,Db.State>) statesEnabledForUser 
    (eventualRejectionsOfOrderItems: Map<int,Db.RejectedOrderItems option>) categories userView (outGroupsOfOrder:Db.OrderOutGroup list) 
    (myOrdersDetails:Db.Orderdetail list) (otherOrdersDetails:Db.Orderdetail list) (user:UserLoggedOnSession) =

    let backUrl = sprintf Path.Orders.viewOrder order.Orderid

    let ingredientsVarOrderItmLink (orderItem:Db.OrderItemDetails) =
        if (mapOfStates.[orderItem.Stateid].Isinitial) then
         a ((sprintf Path.Orders.editOrderItemVariation orderItem.Orderitemid (WebUtility.UrlEncode (backUrl+"#order"+ (orderItem.Orderid |> string ))  ) )) ["",""] [Text local.Ingredients]

        else em ""


    let linksMoveOutGroup = outGroupsOfOrder |> 
        List.map (fun (x:Db.OrderOutGroup) -> ( if (x.Printcount <= 0) then (
                a ((sprintf Path.Orders.moveInitialStateOrderItemsByOutGroup order.Orderid x.Ordergroupid 
                (WebUtility.UrlEncode (sprintf Path.Orders.viewOrder order.Orderid )))) ["class","buttonEnabled"] [Text (local.ConfirmGroup + (x.Groupidentifier|> string )  )]
            ) else  (
                a ((sprintf Path.Orders.reprintOrderItemsGroup order.Orderid x.Ordergroupid 
                (WebUtility.UrlEncode (sprintf Path.Orders.viewOrder order.Orderid )))) ["class","buttonPrinted"] [Text (local.ReprintGroup + (x.Groupidentifier|> string )  )]
            )
        )
        )

    let canRemoveOrderItem = isUserAdmin user

    let canVoidOrder = voidOrderLink order.Orderid userView (Path.Orders.myOrdersSingles)
    let triplesOfGroupMoveLinks = makePairsOfAlist linksMoveOutGroup
    (ordersBar myOrdersDetails otherOrdersDetails)@
    [

      tag "p" [] [a Path.Orders.addSingleOrder ["class","buttonX"] [Text local.NewOrder  ] ]

      ulAttr ["id","item-list"] [
           tag "innerp" [] [
                tag "a" [local.Name,local.Order+((order.Orderid) |> string)] []
                br []
                Text("Tav.:"+order.Table)
                Text(", resp:"+order.Username)

                canVoidOrder
                (a (sprintf Path.Orders.selectOrderFromWhichMoveOrderItems order.Orderid) [] [Text local.Merge])
                br []
                addItemOfCategory order categories backUrl 
                table [
                    for orderItem in orderItems ->
                     tr
                       [
                           td [ 
                               Text(orderItem.Quantity.ToString()+" "+orderItem.Name+" "+orderItem.Comment+" "+ 
                                 (if (not (mapOfStates.[orderItem.Stateid].Isinitial)) then  orderItem.Statusname else ""))
                               modifyOrderItemLink orderItem mapOfStates backUrl
                               deleteOrderItemLink orderItem mapOfStates backUrl
                               ingredientsVarOrderItmLink orderItem
                               removeOrderItemLink orderItem canRemoveOrderItem backUrl

                               (if (orderItem.Hasbeenrejected && ((eventualRejectionsOfOrderItems.[orderItem.Orderitemid]).IsSome)) then 
                                (Text("resp:"+(eventualRejectionsOfOrderItems.[orderItem.Orderitemid].Value).Cause)) else (Text("")))
                           ]

                       ]
                ]
                table
                 ([
                     for blockMoveTriplet in triplesOfGroupMoveLinks ->
                     (
                         (tr 
                            [ for blockMoveItem in blockMoveTriplet ->
                             (
                                 match blockMoveItem with
                                 | Some X -> (td [
                                     X ])
                                 | _ -> td []
                             )
                             ]
                   ))])

                script ["type","text/javascript"; "src","/confirmRemoveOrder.js"] []
            ]
        ]
    ]


let seeDoneOrders  (orders: Db.NonArchivedOrderDetail list) (orderItemsOfOrders: Map<int,Db.OrderItemDetails list>) (ordersHavingSubOrdersMap: Map<int,bool>) =
    [  
      br []
      a (Path.Orders.dearchiveLatestOrder ) ["class","buttonX"] [Text(local.UnArchiveLatest )]
      tag "p" [] [for order in orders -> ( a ("#order"+(order.Orderid |> string)) [] [Text (" Tav. " + order.Table)]) ]
      br []
      br []
      ulAttr ["id","item-list"] [
        for order in orders ->
            let orderItems = orderItemsOfOrders.[order.Orderid]

            tag "innerp" [] [
                br []
                tag "a" ["name","order"+((order.Orderid) |> string)] []
                br []
                Text("tav.:"+order.Table)
                Text(", resp:"+order.Username)
                Text(", totale: "+(string)order.Total)
                (if (order.Total <> order.Adjustedtotal) then Text(local.TotalDiscounted+":"+(string)order.Adjustedtotal) else Text("") )
                a (sprintf  Path.Orders.achiveOrder order.Orderid) ["",""] [Text(local.Archive)]
                a (sprintf  Path.Orders.editDoneOrder order.Orderid) ["",""] [Text(local.Modify)]
                (if (true) then (a (sprintf  Path.Orders.subdivideDoneOrder order.Orderid) ["",""] [Text(local.Subdivide)]) else (Text "")  )
                (if (true) then (a (sprintf  Path.Orders.colapseDoneOrder order.Orderid) ["",""] [Text(local.CollapseOrderItems)]) else (Text "")  )
                (if (not ordersHavingSubOrdersMap.[order.Orderid]) then (a (sprintf  Path.Orders.wholeOrderPaymentItems order.Orderid) ["",""] [Text(local.Receipt)]) else (Text "")  )

                br []
                table [
                    for orderItem in orderItems ->
                     tr
                       [
                       td [ 
                             a (sprintf Path.Orders.editDoneOrderitem orderItem.Orderitemid orderItem.Categoryid) ["",""] 
                              [Text(orderItem.Quantity.ToString()+" "+orderItem.Name+" "+orderItem.Comment+" "+ (string)orderItem.Price
                                + (if (orderItem.Quantity>1) then " * " + (string)(orderItem.Quantity)  +    " = "+(string)((decimal)orderItem.Quantity*orderItem.Price) else "")

                              )
                              ]
                          ]
                       ]
                ]
            ]
      ]
    ]



let editOrderItemVariations (orderItemDetail:Db.OrderItemDetails) (ingredients: Db.IngredientOfCourse list)  
    (existingVariations: Db.VariationDetail list) (ingredientCategories: Db.IngredientCategory list) 
    (ingredientsYouCanAdd: Db.IngredientDetail list) (specificCustomAddQuantitiesForIngredients: Map<int,Db.IngredientPrice list>)  encodedBackUrl  =

    let flatListOfIngredientButtons = (ingredientCategories |> List.map (fun (x:Db.IngredientCategory) -> 
        td [ a ((sprintf Path.Orders.editOrderItemVariationByIngredientCategory orderItemDetail.Orderitemid 
                  x.Ingredientcategoryid (WebUtility.HtmlEncode encodedBackUrl))) ["class","buttonX"]
                  [Text (x.Name)]    ])) @ [ td [  a ((sprintf Path.Orders.editOrderItemVariation orderItemDetail.Orderitemid (WebUtility.UrlEncode encodedBackUrl))) ["class","buttonX"] [Text(local.All)] ]]

    let triplesOfIngredientsList = makePairsOfAlist flatListOfIngredientButtons

    let mappedSpecificCustomAddQuantititesForAddIngredients = 
        specificCustomAddQuantitiesForIngredients |> Map.toList |>  List.map (fun (x,y:Db.IngredientPrice list) -> (x,y |> List.map (fun (z:Db.IngredientPrice) -> (z.Ingredientpriceid, z.Quantity) )))

    let javascriptFormatOfCustomAddQantititesForAddingredients = Utils.intPairsMapToJavascriptString mappedSpecificCustomAddQuantititesForAddIngredients

    let ingredientMap = ingredientsYouCanAdd |> List.map (fun (x:Db.IngredientDetail) -> ((decimal)x.Ingredientid,x.Ingredientname))
    [ 
        tag "h1" [] [Text(local.Variations+orderItemDetail.Name)]
        tag "h2" [] [Text(local.AddIngredient)]

        table [for ingredientCategoryTriple in triplesOfIngredientsList ->
            tr [ for subItem in ingredientCategoryTriple  ->
                match subItem with
                | Some theSubItem ->
                    theSubItem
                | None -> td []
            ]  
        ]

        renderForm
           { Form = Form.addIngredient
             Fieldsets = 
                 [ { Legend = local.IngredientToAdd
                     Fields = 
                         [ 
                           { Label = local.NameBySelection
                             Html =  selectInput 
                                       (fun f -> <@ f.IngredientBySelect  @>) 
                                        ingredientMap (None)}

                           { Label = local.Quantity
                             Html =  selectInput 
                                       (fun f -> <@ f.Quantity  @>) 
                                        pocoNormaleMolto (Some Globals.AGGIUNGINORMALE)}

                           { Label = local.NameByFreeText
                             Html =  formInput 
                                       (fun f -> <@ f.IngredientByText  @>) []}

                                       ] } 
                                       ]
             SubmitText = local.Add }
        Text(sprintf  "%s %.2f." local.OriginalPrice orderItemDetail.Originalprice)
        Text(sprintf  "%s %.2f." local.RecalculatedPrice orderItemDetail.Price)

        tag "fieldset" [] [
            tag "legend" [] [Text(local.ExistingVariations)]
            ulAttr ["id","item-list"] [
                for variation in existingVariations ->
                    let textToDisplay = 
                        match variation.Tipovariazione with
                        | Globals.UNITARY_MEASUSERE -> (variation.Plailnumvariation |> string) + " " + allergeneMarkVariationPrintForDetail(variation) +  " (" + local.Void+ ")"
                        | Globals.PER_PREZZO_INGREDIENTE -> (variation.Quantity |> string) + " " + allergeneMarkVariationPrintForDetail(variation) +  " ("+local.Void+ ") "
                        | _ -> variation.Tipovariazione+ " "+allergeneMarkVariationPrintForDetail(variation) +  " ("+local.Void+")"

                    tag "p" [] 
                            [     
                                  (if (true) then
                                   (a ((sprintf Path.Orders.removeIngredientVariation  variation.Variationsid orderItemDetail.Orderitemid (WebUtility.UrlEncode encodedBackUrl))) ["class","buttonX"] [Text (textToDisplay)])
                                     else (Text(""))
                                  )
                                  (if (variation.Tipovariazione = Globals.UNITARY_MEASUSERE) then
                                   (a ((sprintf Path.Orders.increaseUnitaryIngredientVariation  variation.Variationsid (WebUtility.UrlEncode encodedBackUrl))) ["class","buttonX"] [Text ("+")])
                                     else (Text(""))
                                  )
                                  (if (variation.Tipovariazione = Globals.UNITARY_MEASUSERE && variation.Plailnumvariation > 1) then
                                   (a ((sprintf Path.Orders.decreaseUnitaryIngredientVariation  variation.Variationsid  (WebUtility.UrlEncode encodedBackUrl))) ["class","buttonX"] [Text ("-")])
                                     else (Text(""))
                                  )
                                  (if (variation.Tipovariazione = Globals.UNITARY_MEASUSERE && variation.Plailnumvariation = 1) then
                                   (a ((sprintf Path.Orders.removeIngredientVariation  variation.Variationsid orderItemDetail.Orderitemid  (WebUtility.UrlEncode encodedBackUrl))) ["class","buttonX"] [Text ("-")])
                                     else (Text(""))
                                  )
                            ]
            ]
        ]

        tag "fieldset" [] [
            tag "legend" []  [Text(local.PossibleVariations)]   

            tag "p" []  [
                    a ((sprintf Path.Orders.removeAllAllergenicFromOrderItem orderItemDetail.Orderitemid encodedBackUrl )) ["class","buttonX"] [Text(Globals.ELIMINA_ALLERGENI)]
                    a ((sprintf Path.Orders.removeAllUnavailableIngredientsFromOrderItem orderItemDetail.Orderitemid encodedBackUrl )) ["class","buttonX"] [Text(Globals.ELIMINA_INGREDIENTI_INVISIBILI)]
                ]

            table 
               [ for ingredient in ingredients ->
                let aVariationSenzaExists = existingVariations |> List.exists (fun (x:Db.VariationDetail) -> (x.Ingredientid = ingredient.Ingredientid && x.Tipovariazione = Globals.SENZA))
                let aVariationPocoExists = existingVariations |> List.exists (fun (x:Db.VariationDetail) -> (x.Ingredientid = ingredient.Ingredientid && x.Tipovariazione = Globals.POCO))
                let aVariationMoltoExists = existingVariations |> List.exists (fun (x:Db.VariationDetail) -> (x.Ingredientid = ingredient.Ingredientid && x.Tipovariazione = Globals.MOLTO))

                let attrSenza = if aVariationSenzaExists then [("class","isDisabled");("class","buttonX");("alt","SENZA")] else [("class","buttonX");("alt","SENZA")] 
                let attrPoco = if aVariationPocoExists then [("class","isDisabled");("class","buttonX");("alt","POCO")] else [("class","buttonX");("alt","POCO")] 
                let attrMolto = if aVariationMoltoExists then [("class","isDisabled");("class","buttonX");("alt","ABBONDANTE")] else [("class","buttonX");("alt","ABBONDANTE")] 

                tr [
                    td [
                          Text(allergeneMarkVariationPrintForIngredientOfCourse ingredient)
                    ]

                    td [
                        a (sprintf Path.Orders.addWithougIngredientVariation orderItemDetail.Orderitemid ingredient.Ingredientid ingredient.Ingredientcourseid (WebUtility.UrlEncode  encodedBackUrl)) attrSenza
                          [Text (Globals.SENZA)]
                    ]
                    td [
                        a (sprintf Path.Orders.addDiminuishIngredientVariation orderItemDetail.Orderitemid ingredient.Ingredientid (WebUtility.UrlEncode encodedBackUrl)) attrPoco 
                          [Text (Globals.POCO)]
                    ]
                    td [
                        a (sprintf Path.Orders.addIncreaseIngredientVariation orderItemDetail.Orderitemid ingredient.Ingredientid (WebUtility.UrlEncode encodedBackUrl)) attrMolto 
                          [Text (Globals.MOLTO)]
                    ]
                ]
            ]
        ]
        br []
        tag "p" [] [a (encodedBackUrl) ["class","buttonX"] [Text(local.GoBack)]]
        script [] [Raw("var ingrAdds = "+javascriptFormatOfCustomAddQantititesForAddingredients)]
        script ["type","text/javascript"; "src","/autocompleteEditOrderItemIng.js"] []
    ]

let  askConfirmationVoidOrderByUserLoggedOn orderId encodedBackUrl (user: UserLoggedOnSession) =
    [
        Text("Sicuro che vuoi annullare l'ordine?")
        br []
        a ((sprintf Path.Orders.confirmVoidOrderFromMyOrders orderId encodedBackUrl)) ["class","buttonX"] [Text local.Yes ]
        br []
        br []
        a (( WebUtility.UrlDecode encodedBackUrl)) ["class","buttonX"] [Text local.No ]
    ]


let qrOrder (user:UserLoggedOnSession) (orders: Db.Orderdetail list ) 
    (categories:Db.CourseCategories list) (orderItemsOfOrders: Map<int,Db.OrderItemDetails list>)  
    (mapOfStates: Map<int,Db.State>)   (eventualRejectionsOfOrderItems: Map<int,Db.RejectedOrderItems option>)  = 


    let ctx = Db.getContext()
    let dbUser = Db.tryFindUserById (user.UserId) ctx

    let statesEnabledForUser = Db.listOfEnabledStatesForWaiter user.UserId ctx

    let isUserEnabledForState stateId =
        statesEnabledForUser |> List.contains stateId

    let modifyOrderItemLink (orderItem:Db.OrderItemDetails) =
        if (mapOfStates.[orderItem.Stateid].Isinitial) then
         a ((sprintf Path.Orders.editOrderItemByCategory orderItem.Orderitemid orderItem.Categoryid (WebUtility.UrlEncode Path.Extension.qrUserOrder))) ["",""] [Text local.Modify ]
        else em ""

    let ingredientsVarOrderItmLink (orderItem:Db.OrderItemDetails) =
        if (mapOfStates.[orderItem.Stateid].Isinitial) then
         a ((sprintf Path.Orders.editOrderItemVariation orderItem.Orderitemid (WebUtility.UrlEncode Path.Extension.qrUserOrder))) ["",""] [Text local.Ingredients]
        else em ""

    let goNextStateLink (orderItem:Db.OrderItemDetails) =
        if (isUserEnabledForState orderItem.Stateid)||(mapOfStates.[orderItem.Stateid].Isinitial)
            then
                a ((sprintf Path.Orders.moveOrderItemToTheNextStateAndGoMyOrders orderItem.Orderitemid (WebUtility.UrlEncode (Path.Extension.qrUserOrder+"#"+(orderItem.Orderid |> string)) )) ) ["class","buttonX"] [Text (local.Confirm)]
            else
                em ""

    let voidOrderLink orderId = 
       match dbUser with 
       | Some theUser -> 
          match theUser.Canvoidorders with
            |  true ->   (a (sprintf Path.Orders.voidOrderFromMyOrders orderId (WebUtility.UrlEncode Path.Extension.qrUserOrder) ) ["",""] [Text local.VoidOrder])
            | _ -> em ""
          | _ -> em ""

    [
        Text (user.Username+"  ")
        Text local.ActiveOrders

        br []

        tag "p" [] [for order in orders -> ( a ("#order"+(order.Orderid |> string)) [] [Text (" Tav. " + order.Table)]) 
        ]

        ulAttr ["id", "item-list"] [

            for order in orders  ->           

                  let canVoidOrder = voidOrderLink order.Orderid
                  let orderItems = orderItemsOfOrders.[order.Orderid]
                  let totalprice = List.fold (fun x (y: Db.OrderItemDetails) -> x + (decimal)y.Quantity*(decimal)y.Price) ((decimal)0) orderItems
                  tag "innerp" [] [
                    tag "a" ["name","order"+((order.Orderid) |> string)] []

                    br []
                    Text("ord. n.: "+order.Orderid.ToString())
                    Text(", nome:"+order.Person) 
                    Text(", tav.:"+order.Table)
                    Text(", resp:"+order.Username)
                    Text(", totale:"+totalprice.ToString())
                    canVoidOrder
                    br []
                    addItemOfCategory order categories Path.Extension.qrUserOrder
                    table 
                     [
                     for orderItem in orderItems  -> 
                     tr
                      [
                       td [ 
                       Text(orderItem.Quantity.ToString()+" "+orderItem.Name+" "+orderItem.Comment+" "+ 
                         (if (not (mapOfStates.[orderItem.Stateid].Isinitial)) then  orderItem.Statusname else ""))
                       modifyOrderItemLink orderItem
                       deleteOrderItemLink orderItem mapOfStates (WebUtility.UrlEncode (Path.Extension.qrUserOrder))
                       ingredientsVarOrderItmLink orderItem

                       (if (orderItem.Hasbeenrejected && ((eventualRejectionsOfOrderItems.[orderItem.Orderitemid]).IsSome)) then 

                        (Text("resp:"+(eventualRejectionsOfOrderItems.[orderItem.Orderitemid].Value).Cause)) else (Text("")))

                       ]
                      ]
                    ]
                ]
        ]
        script ["type","text/javascript"; "src","/autorefresh.js"] []
    ]

let seeVariationOfIngredientStartingFromDate (ingredient:Db.Ingredient)   date (ingredientDecrements: Db.IngredientDecrementView list) =
    [
        div [][
            div [][h2 (local.SeeIngredientUsages + ingredient.Name)]
            div [][
                for ingredientDecrement in ingredientDecrements -> 
                tag "p" [] [(Text("n. "+(ingredientDecrement.Numberofcourses |> string) + 
                    ", "+local.Course+":"+ ingredientDecrement.Coursename+" "+
                    (ingredientDecrement.Closingtime |> string) + local.OrdinaryQuantity+(ingredientDecrement.Ingredientquantity |> string)+ ingredientDecrement.Unitmeasure + " var. :"+ingredientDecrement.Typeofdecrement)) ]
            ]
        ]
    ]

let chooseDateForDecrementHistory ingredientId  =
    [
        h2 (local.SeeComsumptionStartingFrom  )
        br[]
        renderForm 
            { 
                  Form = Form.date
                  Fieldsets =
                       [
                          { Legend = local.StartingFrom
                            Fields =
                                [
                                    {
                                        Label = "Data (AA/MM/GG)"
                                        Html = formInput (fun f -> <@ f.Date @>) []
                                    }
                                ]
                          }
                       ]
                  SubmitText = local.Visualize
            }
    ]


let editIngredientPrices (ingredient: Db.Ingredient) (ingredientPrices: Db.IngredientPriceDetail list) msg=
    let ingredientCategoryId = ingredient.Ingredientcategoryid
    [
        h2(local.AddIngredientPrice + ingredient.Name + local.MeasuringSystem + ingredient.Unitmeasure)
        h2(msg)
        renderForm 
                { 
                  Form = Form.ingredientPrice
                  Fieldsets =
                    [ { Legend = local.Add
                        Fields = 
                            [
                                {
                                    Label = local.AddPrice
                                    Html = formInput (fun f -> <@ f.AddPrice @>) []
                                }
                                {
                                    Label = local.SubtractPrice
                                    Html = formInput (fun f -> <@ f.SubtractPrice @>) []
                                }
                                {
                                    Label = local.Quantity
                                    Html = formInput (fun f -> <@ f.Quantity @>) []
                                }
                                {
                                    Label = local.IsDefaultAddQuantity
                                    Html = selectInput (fun f -> <@ f.IsDefaultAdd @>) yesOrNo (None)  
                                }
                                {
                                    Label = local.IsDefaultSubtractQuantity
                                    Html = selectInput (fun f -> <@ f.IsDefaultSubtract @>) yesOrNo (None)
                                }
                            ]
                      }
                    ]
                  SubmitText = local.Add
                }
        br []
        table [
            for ingredientPrice in ingredientPrices ->
            let isDefaultAdd = ingredientPrice.Isdefaultadd
            let isDefaultSubtract = ingredientPrice.Isdefaultsubtract
            let textToDisplay = 
                match (isDefaultAdd,isDefaultSubtract) with
                | (true,true) -> local.IsDefaultAddAndSubtractQuantity+" "+local.Remove 
                | (true,false) -> local.IsDefaultAddQuantity+" "+local.Remove
                | (false,true) -> local.IsDefaultSubtractQuantity+" "+local.Remove
                | (false,false) -> local.Remove

            tr [
                td [
                    Text(sprintf "%s (%s) %.2f" local.Quantity ingredient.Unitmeasure ingredientPrice.Quantity)
                ]
                td [
                    Text(sprintf "%s %.2f" local.AddPrice ingredientPrice.Addprice)
                ]
                td [
                    Text(sprintf "%s %.2f" local.SubtractPrice ingredientPrice.Subtractprice)
                ]

                td [
                    (a (sprintf Path.Admin.deleteIngredientPrice ingredientPrice.Ingredientpriceid) ["class","buttonX"] [Text(textToDisplay)]) 
                ]
            ]
        ]
        br []
        a (sprintf Path.Admin.editIngredientCategoryPaginated ingredientCategoryId 0 ) [] [Text(local.GoBack)]
    ]

let seeIngredientsOfACategoryPaginated (category:Db.IngredientCategory) (allIngredientOfCategory:Db.Ingredient list) numberOfPages pageNumber =
    let nextPageLink ingCatId i = if (i<numberOfPages) then [a (sprintf Path.Admin.editIngredientCategoryPaginated ingCatId (i+1)) ["class","noredstyle"] [Text (">")]] else []
    let previosPageLink ingCatId i = if (i>0) then [a (sprintf Path.Admin.editIngredientCategoryPaginated ingCatId (i - 1)) ["class","noredstyle"] [Text ("<")]] else []
    
    [
        h2 (local.AllIngredientsOfCategory+": " + category.Name)

        a (sprintf Path.Admin.editIngredientCategory category.Ingredientcategoryid) ["class","buttonX"] [Text(local.AddNew)]

        br []
        renderForm 
                { Form = Form.searchCourse
                  Fieldsets =
                    [ { Legend = local.SearchByName 
                        Fields = 
                            [
                                {
                                    Label = local.Name
                                    Html = formInput (fun f -> <@ f.Name @>) []
                                }
                            ]
                      }
                      ]
                  SubmitText = local.Search
                }

        br []
        table [
            for ingredient in allIngredientOfCategory ->
            tr [
                td [
                   a (sprintf Path.Admin.editIngredient ingredient.Ingredientid pageNumber)  ["class",(if ingredient.Visibility then "buttonXSmallSizeFont" else "buttonYSmallSizeFont")] [Text(ingredient.Name)]
                ]
                td [
                   a (sprintf Path.Admin.editIngredientPrices ingredient.Ingredientid)  ["class","buttonX"] [Text("prezzi")]
                ]
                td [
                    (if (ingredient.Checkavailabilityflag) then
                        (div [] [ Text (ingredient.Availablequantity |> string)])
                    else 
                         a (sprintf Path.Admin.viewIngredientUsage ingredient.Ingredientid pageNumber)  ["class",(if ingredient.Visibility then "buttonX" else "buttonY")] [Text(local.ViewUsages)]
                   )
                ]
                td [
                   a (sprintf Path.Admin.fillIngredient ingredient.Ingredientid pageNumber)  ["class","buttonX"] [Text(local.Load)]
                ]
            ]
        ]
        div []  ((previosPageLink category.Ingredientcategoryid pageNumber) @  ([0 .. numberOfPages] |> (List.map (fun i -> 

            if (i = pageNumber) then
            ((a (sprintf Path.Admin.editIngredientCategoryPaginated category.Ingredientcategoryid i)) ["class","redstyle"] [Text ((i |> string)+" ")])
            else 
            ((a (sprintf Path.Admin.editIngredientCategoryPaginated category.Ingredientcategoryid i)) ["class","noredstyle"] [Text ((i |> string)+" ")]))
        )) @ nextPageLink category.Ingredientcategoryid pageNumber)
        br []
        br []
        (a  Path.Admin.allIngredientCategories [] [Text(local.GoBack)])
    ]

let ordersListbySingles (userView: Db.UsersView)  (myOrders: Db.Orderdetail list ) (otherOrders: Db.Orderdetail list)   
    (mapOfStates: Map<int,Db.State>) statesEnabledForUser backUrl  initStateId (outGroupsOfOrders: Map<int,Db.OrderOutGroup list>)  =

    let someOrderItemsAreAtInitialState (orderItems:  Db.OrderItemDetails List) = 
        (orderItems |> List.tryFind (fun (x:OrderItemDetails) -> x.Stateid = initStateId)).IsSome

    let isUserEnabledForState stateId =
        statesEnabledForUser |> List.contains stateId

    let ingredientsVarOrderItmLink (orderItem:Db.OrderItemDetails) =
        if (mapOfStates.[orderItem.Stateid].Isinitial) then
         a ((sprintf Path.Orders.editOrderItemVariation orderItem.Orderitemid (WebUtility.UrlEncode (backUrl+"#order"+ (orderItem.Orderid |> string ))  ) )) ["",""] [Text local.Ingredients]

        else em ""
    [
        Text (userView.Username+"  ")
        Text local.ActiveOrders

        br []

    ]@(ordersBar myOrders otherOrders ) @
      [
        tag "p" [] [a Path.Orders.addSingleOrder ["class","buttonX"] [Text local.NewOrder] ]
        script ["type","text/javascript"; "src","/autorefresh.js"] []
      ]

let ordersList (userView: Db.UsersView)  (orders: Db.Orderdetail list ) 
    (categories:Db.CourseCategories list) (orderItemsOfOrders: Map<int,Db.OrderItemDetails list>)  
    (mapOfStates: Map<int,Db.State>) statesEnabledForUser backUrl (eventualRejectionsOfOrderItems: Map<int,Db.RejectedOrderItems option> ) initStateId (outGroupsOfOrders: Map<int,Db.OrderOutGroup list>)  =

    let someOrderItemsAreAtInitialState (orderItems:  Db.OrderItemDetails List) = 
        (orderItems |> List.tryFind (fun (x:OrderItemDetails) -> x.Stateid = initStateId)).IsSome

    let isUserEnabledForState stateId =
        statesEnabledForUser |> List.contains stateId

    let ingredientsVarOrderItmLink (orderItem:Db.OrderItemDetails) =
        if (mapOfStates.[orderItem.Stateid].Isinitial) then
         a ((sprintf Path.Orders.editOrderItemVariation orderItem.Orderitemid (WebUtility.UrlEncode (backUrl+"#order"+ (orderItem.Orderid |> string ))  ) )) ["",""] [Text local.Ingredients]

        else em ""

    [
        Text (userView.Username+"  ")
        Text local.ActiveOrders

        br []

        tag "p" [] [
            for order in orders -> ( a ("#order"+(order.Orderid |> string)) [] [Text (" Tav. " + order.Table)]) 
        ]

        tag "p" [] [
            for order in orders -> ( a (sprintf Path.Orders.viewOrder order.Orderid)  [] [Text (" Tav. " + order.Table)]) 
        ]

        tag "p" [] [a Path.Orders.addOrder ["class","buttonX"] [Text local.NewOrder ] ]
        unVoidMyLatest userView backUrl
        allOrdersLinkForUserView userView
        myOrdersLink userView

        ulAttr ["id", "item-list"] [

            for order in orders  ->           
                  let canVoidOrder = voidOrderLink order.Orderid userView backUrl
                  let orderItems = orderItemsOfOrders.[order.Orderid]
                //   let totalprice = List.fold (fun x (y: Db.OrderItemDetails) -> x + (decimal)y.Quantity*(decimal)y.Price) ((decimal)0) orderItems

                  let linksMoveOutGroup = outGroupsOfOrders.[order.Orderid] |> 
                        List.map (fun (x:Db.OrderOutGroup) -> ( if (x.Printcount <= 0) then (
                                a ((sprintf Path.Orders.moveInitialStateOrderItemsByOutGroup order.Orderid x.Ordergroupid 
                                (WebUtility.UrlEncode (WebUtility.UrlEncode (backUrl+"#order"+ (order.Orderid |> string ))) ))) ["class","buttonEnabled"] [Text (local.ConfirmGroup+(x.Groupidentifier|> string )  )]
                            ) else  (
                                a ((sprintf Path.Orders.reprintOrderItemsGroup order.Orderid x.Ordergroupid 
                                (WebUtility.UrlEncode ((WebUtility.UrlEncode (backUrl+"#order"+ (order.Orderid |> string ))))))) ["class","buttonPrinted"] [Text (local.ReprintGroup + (x.Groupidentifier|> string )  )]

                            )
                        )
                        )


                  let triplesOfGroupMoveLinks = makePairsOfAlist linksMoveOutGroup

                  tag "innerp" [] [
                    tag "a" ["name","order"+((order.Orderid) |> string)] []
                    br []
                    Text("tav.:"+order.Table)
                    canVoidOrder
                    br []
                    addItemOfCategory order categories backUrl
                    table 
                     ([
                         for orderItem in orderItems  -> 
                         tr
                              [
                                   td [ 
                                       Text(orderItem.Quantity.ToString()+" "+orderItem.Name+" "+orderItem.Comment+" "+ 
                                         (if (not (mapOfStates.[orderItem.Stateid].Isinitial)) then  orderItem.Statusname else ""))
                                       modifyOrderItemLink orderItem mapOfStates backUrl
                                       deleteOrderItemLink orderItem mapOfStates backUrl
                                       ingredientsVarOrderItmLink orderItem

                                       (if (orderItem.Hasbeenrejected && ((eventualRejectionsOfOrderItems.[orderItem.Orderitemid]).IsSome)) then 
                                        (Text("resp:"+(eventualRejectionsOfOrderItems.[orderItem.Orderitemid].Value).Cause)) else (Text("")))
                                   ]
                              ] 
                     ])

                    table
                     ([
                         for blockMoveTriplet in triplesOfGroupMoveLinks ->
                         (
                             (tr 
                                [ for blockMoveItem in blockMoveTriplet ->
                                 (
                                     match blockMoveItem with
                                     | Some X -> (td [
                                         X ])
                                     | _ -> td []
                                 )
                                 ]
                       ))])

                    br []
                ]
        ]
        script ["type","text/javascript"; "src","/autorefresh.js"] []
    ]


    
   