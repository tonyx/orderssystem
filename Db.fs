
module OrdersSystem.Db

open FSharp.Data
open FSharp.Data.Sql
open OrdersSystem
open System
open System.Data
open System.Globalization

// open Microsoft.FSharp.Linq.Nullable

let log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


[<Literal>]
let TPConnectionString = 
    "Server=127.0.0.1;"    + 
    "Database=mrbilly;" + 
    "User Id=suave;"            + 
    "Password=1234;"

let [<Literal>] dbVendor = Common.DatabaseProviderTypes.POSTGRESQL
let [<Literal>] connexStringName = "DefaultConnectionString"

let [<Literal>] resPath = "/Users/Tonyx/Projects/mrbilly6/OrdersSystem/packages/Npgsql.2.2.1/lib/net45/"  // location of a valid Npgsql.dll file used at compile time by sql entity framework
let [<Literal>] indivAmount = 1000
let [<Literal>] useOptTypes  = false


type Sql =
    SqlDataProvider< 
        dbVendor,
        TPConnectionString,
        "",        
        resPath,
        indivAmount,
        useOptTypes>

type Visibility = VISIBLE| INVISIBLE

let getVisibilityToken x =
    match x with 
    | VISIBLE -> "VISIBLE"
    | INVISIBLE -> "INVISIBLE"



type DbContext = Sql.dataContext
type Course = DbContext.``public.coursesEntity``
type User = DbContext.``public.usersEntity``
type Order = DbContext.``public.ordersEntity``
type OrderItem = DbContext.``public.orderitemsEntity``
type OrderItemDetails = DbContext.``public.orderitemdetailsEntity``
type CourseCategories = DbContext.``public.coursecategoriesEntity``
type Coursedetails = DbContext.``public.coursedetails2Entity``
type Role =  DbContext.``public.rolesEntity``
type UsersView = DbContext.``public.usersviewEntity``
type State = DbContext.``public.statesEntity``
type Observer = DbContext.``public.observersEntity``
type Enabler = DbContext.``public.enablersEntity``
type ObserverRoleStatusCategory = DbContext.``public.observersrolestatuscategoriesEntity``
type EnablerRoleStatusCategory = DbContext.``public.enablersrolestatuscategoriesEntity``
type OrderItemState = DbContext.``public.orderitemstatesEntity``
type DefaultActionableState = DbContext.``public.defaultactionablestatesEntity``
type WaiterActionableState = DbContext.``public.waiteractionablestatesEntity``
type Orderdetail = DbContext.``public.orderdetailsEntity``
type IngredientCategory = DbContext.``public.ingredientcategoryEntity``
type Ingredient = DbContext.``public.ingredientEntity``
type IngredientDetail = DbContext.``public.ingredientdetailsEntity``
type IngredientOfCourse = DbContext.``public.ingredientofcoursesEntity``
type Variation = DbContext.``public.variationsEntity``
type VariationDetail = DbContext.``public.variationdetailsEntity``
type IngredientCourse = DbContext.``public.ingredientcourseEntity``
type ArchivedorderLogBuffer = DbContext.``public.archivedorderslogbufferEntity``
type VoidedOrdersLogBuffer = DbContext.``public.voidedorderslogbufferEntity``
type TempUserDefaultActionableStates = DbContext.``public.temp_user_default_actionable_statesEntity``
type RejectedOrderItems = DbContext.``public.rejectedorderitemsEntity``
type IngredientIncrement = DbContext.``public.ingredientincrementEntity``
type IngredientDecrement = DbContext.``public.ingredientdecrementEntity``
type IngredientDecrementView = DbContext.``public.ingredientdecrementviewEntity``
type SubOrder = DbContext.``public.suborderEntity``
type OrderOutGroup = DbContext.``public.orderoutgroupEntity``
type OrderOutGroupDetail = DbContext.``public.orderoutgroupdetailsEntity``
type Printer = DbContext.``public.printersEntity``
type PrinterForCourseCategory  = DbContext.``public.printerforcategoryEntity``
type PrinterForCourseCategoryDetail = DbContext.``public.printerforcategorydetailEntity``
type IngredientPrice = DbContext.``public.ingredientpriceEntity``
type IngredientPriceDetail = DbContext.``public.ingredientpricedetailsEntity``
type PrinterForReceiptAndInvoice  = DbContext.``public.printerforreceiptandinvoiceEntity``
type Invoice = DbContext.``public.invoicesEntity``
type CustomerData = DbContext.``public.customerdataEntity``
type NonEmptyOrderDetail = DbContext.``public.nonemptyorderdetailsEntity``
type TenderCode = DbContext.``public.tendercodesEntity``
type PaymentItem = DbContext.``public.paymentitemEntity``
type NonArchivedOrderDetail = DbContext.``public.nonarchivedorderdetailsEntity``
type PaymentItemDetail = DbContext.``public.paymentitemdetailsEntity``


let getContext() = Sql.GetDataContext(TPConnectionString)

module Courses = 
    let tryFindCourseByName name (ctx: DbContext) =
        query {
            for course in ctx.Public.Courses do
                where (course.Name =% (name+"%"))
                select course
            } |> Seq.tryHead

    let getCourse courseId (ctx:DbContext) =
        log.Debug(sprintf "%s %d" "getCourse" courseId)
        ctx.Public.Courses |> Seq.find (fun (x:Course) -> x.Courseid = courseId)

    let tryGetCourseByName courseName (ctx:DbContext) =
        query {
            for course in ctx.Public.Courses do 
            where (course.Name = courseName)
            select course
        } |> Seq.tryHead

    let tryFindCourseById id (ctx: DbContext) =
        log.Debug(sprintf "%s %d" "tryFindCourseById" id)
        query {
            for course in ctx.Public.Courses do
                where (course.Courseid = id)
                select course
        } |> Seq.tryHead

    let getAllCourses (ctx: DbContext) : Course list =
        log.Debug("getAllCourses")
        query {
            for course in ctx.Public.Courses do
                sortBy course.Name
                select course
        } |> Seq.toList

    let getVisibleCoursesDetailsByCategoryAndPage categoryId pageId (ctx: DbContext): Coursedetails list =
        log.Debug(sprintf "%s %d %d" "getVisibleCoursesDetailsByCategoryAndPage" categoryId pageId)
        let startIndex = pageId * Globals.NUM_DB_ITEMS_IN_A_PAGE
        let upperIndex = startIndex + Globals.NUM_DB_ITEMS_IN_A_PAGE 
        query {
                    for corseDetails in ctx.Public.Coursedetails2  do
                        where (categoryId = corseDetails.Categoryid && corseDetails.Visibility)
                        sortBy corseDetails.Name 
                        skip (pageId*Globals.NUM_DB_ITEMS_IN_A_PAGE)
                        take (upperIndex - startIndex)
                        select corseDetails
        } |> Seq.toList

    let getAllCourseDetails (ctx: DbContext) : Coursedetails list =
        log.Debug("getAllCourseDetails")
        query {
            for corseDetails in ctx.Public.Coursedetails2  do
                sortBy corseDetails.Courseid
                select corseDetails
        } |> Seq.toList

    let getAllCoursesDetailsByCatgory categoryId (ctx: DbContext) : Coursedetails list =
        query {
            for corseDetails in ctx.Public.Coursedetails2  do
                where (categoryId = corseDetails.Categoryid)
                sortBy corseDetails.Name
                select corseDetails
        } |> Seq.toList

    let getNumberOfAllCourses (ctx: DbContext) =
        ctx.Public.Courses |> Seq.length

    let getNumberOfAllCoursesOfACategory categoryId (ctx: DbContext) =
        ctx.Public.Courses |> Seq.filter (fun (x:Course) -> (x.Categoryid = categoryId)) |> Seq.length

    let getNumberOfVisibleCoursesOfACatgory categoryId (ctx: DbContext) =
        ctx.Public.Courses |> Seq.filter (fun (x:Course) -> (x.Categoryid = categoryId ) && x.Visibility) |>  Seq.length

    let getAllCoursesDetailsByCategoryAndPage categoryId pageId (ctx: DbContext): Coursedetails list =
        log.Debug(sprintf "%s %d %d" "getAllCoursesDetailsByCategoryAndPage" categoryId pageId)
        let allCourses =
            query {
                    for corseDetails in ctx.Public.Coursedetails2  do
                        where (categoryId = corseDetails.Categoryid)
                        sortBy corseDetails.Name
                        select corseDetails

            } |> Seq.toArray
        let startIndex = pageId * Globals.NUM_DB_ITEMS_IN_A_PAGE
        let upperIndex = min (allCourses.Length - 1) (startIndex + Globals.NUM_DB_ITEMS_IN_A_PAGE - 1)
        if (startIndex < allCourses.Length) then [startIndex .. upperIndex ] |> List.map (fun i -> allCourses.[i]) 
            else []

    let getAllCoursesDetailsByCategoryWithTextNameSearch categoryId (name:string) (ctx: DbContext): Coursedetails list =
        log.Debug(sprintf "%s %d %s " "getAllCoursesDetailsByCategoryWithTextNameSearch" categoryId name)
        let allCourses =
            query {
                    for corseDetails in ctx.Public.Coursedetails2  do
                        where (categoryId = corseDetails.Categoryid && corseDetails.Name.ToLower().Contains(name.ToLower()))
                        sortBy corseDetails.Courseid
                        select corseDetails

            } |> Seq.toArray
        let startIndex = 0 
        let upperIndex = min (allCourses.Length - 1) (startIndex + Globals.NUM_DB_ITEMS_IN_A_PAGE - 1)
        if (startIndex < allCourses.Length) then [startIndex .. upperIndex ] |> List.map (fun i -> allCourses.[i]) 
            else []

    let tryGetCategoryById id (ctx: DbContext) : CourseCategories option =
        log.Debug(sprintf "%s %d" "tryGetCategoryById" id)
        query {
            for category in ctx.Public.Coursecategories do  
                where (category.Categoryid = id )
                select category
        } |> Seq.tryHead

    let getVisibleCoursesDetailsByCatgory categoryId (ctx: DbContext) : DbContext.``public.coursedetails2Entity`` list =
        log.Debug(sprintf "%s %d" "getVisibleCoursesDetailsByCatgory" categoryId)
        query {
            for corseDetails in ctx.Public.Coursedetails2  do
                where (categoryId = corseDetails.Categoryid && corseDetails.Visibility)
                sortBy corseDetails.Courseid
                select corseDetails
        } |> Seq.toList

    let getAllAbstractCourses (ctx:DbContext) =
        log.Debug("getAllAbstractCourses")
        query {
            for course in ctx.Public.Courses do
                join category in ctx.Public.Coursecategories on (course.Categoryid = category.Categoryid)
                where (category.Abstract = true)
                select course
        } |> Seq.toList

    let getAllCategories (ctx: DbContext) : CourseCategories list =
        log.Debug("getAllCategories")
        query {
            for category in ctx.Public.Coursecategories do
                sortBy category.Name
                select category
        } |> Seq.toList

    let getVisibleCourseCategories (ctx: DbContext) : CourseCategories list =
        log.Debug("getVisibleCourseCategories")
        query {
            for category in ctx.Public.Coursecategories do
                where (category.Visibile)
                sortBy category.Categoryid
                select category
        } |> Seq.toList

    let getVisibleCoursesOfACategory categoryId (ctx:DbContext): Course list =
        log.Debug(sprintf "%s %d" "getVisibleCoursesOfACategory" categoryId)
        query {
            for course in ctx.Public.Courses do
                where (course.Categoryid = categoryId && course.Visibility)
                sortBy course.Courseid
                select course
        } |> Seq.toList

    let getVisibleCourses (ctx: DbContext): Course list =
        log.Debug(sprintf "%s" "getVisibleCourses")
        query {
            for course in ctx.Public.Courses do
                where (course.Visibility)
                sortBy course.Courseid
                select course
        } |> Seq.toList

    let getVisibleCoursesByCategory categoryId (ctx: DbContext): Course list =
        log.Debug(sprintf "%s %d" "getVisibleCoursesByCategory" categoryId)
        query {
            for course in ctx.Public.Courses do
                where (course.Visibility && course.Categoryid = categoryId)
                sortBy course.Name
                select course
        } |> Seq.toList

    let createCourse price name  description  visibility  categoryId (ctx: DbContext) =
        log.Debug(sprintf "%s %.2f %s %s %b %d " "createCourse" price name description visibility categoryId)
        let newCourse = ctx.Public.Courses.Create(categoryId,name,visibility)
        newCourse.Description <- description
        newCourse.Price <- price
        ctx.SubmitUpdates()
        newCourse

    let updateCourse (course : Course) price name description visibility categoryId  (ctx : DbContext) =
       log.Debug(sprintf "%s %d" "updateCourse" course.Courseid)
       course.Visibility <- visibility
       course.Name <- name
       course.Description <- description
       course.Price <- price
       course.Categoryid <- categoryId
       ctx.SubmitUpdates()
 
    let tryGetCourse courseId (ctx: DbContext) =
        log.Debug(sprintf "%s %d" "tryGetCourse" courseId )
        query {
            for course in ctx.Public.Courses do
                where (course.Courseid = courseId)
                select course
        } |> Seq.tryHead

    let tryGetCourseCategoryByName name (ctx: DbContext) =
        log.Debug(sprintf "%s %s " "tryGetCourseCategoryByName" name)
        query {
            for category in ctx.Public.Coursecategories do
                where (category.Name = name)
                select category
        } |> Seq.tryHead

    let tryFindCourse id (ctx: DbContext ) =
        log.Debug(sprintf "%s %d" "tryFindCourse" id)
        query {
            for course in ctx.Public.Courses do
                where (course.Courseid=id)
                select course
        } |> Seq.tryHead

    let tryGetCourseCategory id (ctx: DbContext ) =
        log.Debug(sprintf "%s %d " "tryGetCourseCategory" id)
        query {
            for category in ctx.Public.Coursecategories  do
                where (category.Categoryid=id)
                select category
        } |> Seq.tryHead

    let tryFindCategoryByName categoryName (ctx: DbContext) : CourseCategories option =
        log.Debug(sprintf "%s %s" "tryFindCategoryByName" categoryName )
        query {
            for category in ctx.Public.Coursecategories do
            where (category.Name = categoryName)
            select category
        } |> Seq.tryHead

    let getAllourseCategories (ctx: DbContext) =
        log.Debug("getAllourseCategories")
        ctx.Public.Coursecategories |> Seq.toList

    let getActiveCategories (ctx: DbContext): CourseCategories list =
        log.Debug("getActiveCategories")
        ctx.Public.Coursecategories |> Seq.toList |> List.filter (fun (x:CourseCategories) -> x.Visibile)

    let getActiveConcreteCategories (ctx: DbContext): CourseCategories list =
        log.Debug("getActiveCategories")
        ctx.Public.Coursecategories |> Seq.toList |> List.filter (fun (x:CourseCategories) -> x.Visibile && not x.Abstract)



module States =

    let getStateByName stateName (ctx: DbContext) =
        log.Debug(sprintf "%s %s" "getStateByName" stateName)
        ctx.Public.States |> Seq.find (fun (x:State) -> x.Statusname = stateName)

    let getAllStates (ctx:DbContext) =
        ctx.Public.States |> Seq.toList

    let getOrdinaryStates (ctx: DbContext): State list =
        ctx.Public.States |>  Seq.filter (fun (x:State) -> (not x.Isexceptional) ) |> Seq.toList

    let getFinalState (ctx:DbContext) =
        ctx.Public.States |> Seq.find( fun (x:State) -> x.Isfinal)
    let getState id (ctx: DbContext) =
        log.Debug(sprintf "%s %d" "getState" id)
        ctx.Public.States |> Seq.find (fun (x:State) -> x.Stateid = id)

    let tryGetStateByName statName (ctx:DbContext) =
        ctx.Public.States |> Seq.tryFind (fun (x:State)-> x.Statusname = statName) 

    let getInitState (ctx:DbContext): State =
        log.Debug("getInitState")
        let initState =
            query {
                for state in ctx.Public.States do
                    where state.Isinitial
                    select  state
            } |> Seq.tryHead
        match initState with
        Some X -> X | _ -> failwith "error database. set an unique initial state"




module Orders =

    let getTableOfOrder orderId (ctx:DbContext) =
        log.Debug(sprintf "%s %d" "getTableOfOrder" orderId)
        query {
            for order in ctx.Public.Orders do
                where (order.Orderid = orderId) 
                select order.Table
                
        } |> Seq.head
        
    let getOrderItemDetail orderItemId (ctx: DbContext) =
        log.Debug("getOrderItemDetail")
        ctx.Public.Orderitemdetails |> Seq.find (fun (x:OrderItemDetails)-> x.Orderitemid = orderItemId)

    let setOrderAsDone (order: Order) (ctx: DbContext ) =
        log.Debug(sprintf "%s %d" "setOrderAsDone" order.Orderid)
        order.Ongoing <- false;
        ctx.SubmitUpdates()

    let getOrderItemsOfOrderItemThatAreNotInInitalState orderId (ctx:DbContext) =
        log.Debug("getOrderItemsOfO
        rderItemThatAreNotInInitalState")
        let initState = States.getInitState ctx
        query {
            for orderItem in ctx.Public.Orderitems do
                where (orderItem.Orderid = orderId && orderItem.Stateid <> initState.Stateid)
                select orderItem
        } |> Seq.toList
    let getVoidedOrders (ctx:DbContext) =
        ctx.Public.Orders |> Seq.filter (fun (x:Order) -> x.Voided) |> Seq.toList

    let getPayableOrderDetails (ctx: DbContext) =
        log.Debug("getPayableOrderDetails")
        query {
            for order in ctx.Public.Nonarchivedorderdetails do
                sortBy order.Startingtime
        } |> Seq.toList

    let getOrderItemById orderItemId (ctx: DbContext) =
        log.Debug(sprintf "%s %d" "getOrderItemById" orderItemId)
        query {
            for orderItem in ctx.Public.Orderitems do
                where (orderItem.Orderitemid = orderItemId)
                select orderItem
        } |> Seq.tryHead

    let tryGetOrder orderId (ctx: DbContext): Order option =
        log.Debug(sprintf "%s %d" "tryGetOrder" orderId)
        query {
            for order in ctx.Public.Orders do
                where (order.Orderid=orderId)
                select order
        } |> Seq.tryHead

    let setOrderAsDoneById (orderId: int) (ctx: DbContext) =
            log.Debug(sprintf "%s %d" "setOrderAsDoneById" orderId)
            let order = tryGetOrder orderId ctx
            match order with
                | Some X ->  setOrderAsDone X ctx
                | None -> failwith ("unexisting order with id "+orderId.ToString())

    let tryGetOrderItemDetail (orderItemId:int) (ctx:DbContext) =
        log.Debug(sprintf "%s %d" "tryGetOrderItemDetail" orderItemId)
        query {
            for orderItemDetail in ctx.Public.Orderitemdetails do
                where (orderItemDetail.Orderitemid= orderItemId)
                select orderItemDetail
        } |> Seq.tryHead

    let getOrderItemsOfOrder orderId (ctx: DbContext) =
        log.Debug(sprintf "%s %d" "getOrderItemsOfOrder" orderId)
        query {
            for orderItem in ctx.Public.Orderitems do
                where (orderItem.Orderid = orderId)
                select orderItem
        } |> Seq.toList

    let getOngoingOrdersByUser userId (ctx: DbContext) : Order list =
        log.Debug(sprintf "%s %d" "getOngoingOrdersByUser" userId)
        query {
            for order in ctx.Public.Orders do
                where (order.Ongoing && order.Userid = userId && order.Voided = false )
                sortByDescending order.Orderid
                select order
        } |> Seq.toList 

    let getOngoingOrderDetailsByUser userId (ctx: DbContext) : Orderdetail list =
        log.Debug(sprintf "%s %d" "getOngoingOrderDetailsByUser" userId)
        query {
            for order in ctx.Public.Orderdetails do
                where (order.Ongoing && order.Userid = userId && order.Voided = false && order.Archived = false)
                sortByDescending order.Orderid
                select order
        } |> Seq.toList 

    let getOngoingOrderDetailsByAllUserExcept userId (ctx: DbContext) : Orderdetail list =
        log.Debug(sprintf "%s %d" "getOngoingOrderDetailsByAllUserExcept" userId)
        query {
            for order in ctx.Public.Orderdetails do
                where (order.Ongoing && order.Userid <> userId && order.Voided = false && order.Archived = false)
                sortByDescending order.Orderid
                select order
        } |> Seq.toList 

    let getAllUnarchivedAndUnvoidedOngoingOrderDetails userId (ctx: DbContext) : Orderdetail list =
        log.Debug(sprintf "%s %d" "getOngoingOrderDetailsByAllUserExcept" userId)
        query {
            for order in ctx.Public.Orderdetails do
                where (order.Ongoing && order.Userid <> userId && order.Voided = false && order.Archived = false)
                sortByDescending order.Orderid
                select order
        } |> Seq.toList 
    let getOngoingOrderdetails (ctx: DbContext) : Orderdetail list =
        log.Debug("getOngoingOrderdetails")
        query {
            for order in ctx.Public.Orderdetails do
                where (order.Ongoing && (order.Voided = false) && (order.Archived = false))
                sortBy order.Orderid
                select order
        } |> Seq.toList 

    let getOrderItemDetailOfOrdegetOrderItemDetailOfOrderr (order: Order) (ctx: DbContext): OrderItemDetails list  =
        log.Debug(sprintf "%s %d" "getOrderItemDetailOfOrder" order.Orderid)
        query {
            for orderItem in ctx.Public.Orderitemdetails do
                where (orderItem.Orderid = order.Orderid)
                sortBy (orderItem.Startingtime)
                select orderItem
            } |> Seq.toList

    let getAllUnarchivedOrders (ctx:DbContext) =
        log.Debug("getAllUnarchivedOrders")
        query {
           for order in ctx.Public.Orders do
           where (order.Archived = false && order.Voided = false)
           sortBy order.Table
           select order
        } |> Seq.toList

    let getSubOrdersOfOrder (order:Order) (ctx: DbContext) =
        order.``public.suborder by orderid`` |> Seq.toList

    let getOrder orderId (ctx: DbContext)  =
        log.Debug(sprintf "%s %d" "getorder" orderId)
        ctx.Public.Orders |> Seq.find (fun (x: Order) -> x.Orderid = orderId)     

    let createSubOrderOfOrderItem orderId (ctx: DbContext) =
        let subOrder = ctx.Public.Suborder.``Create(creationtime, orderid, payed, subtotal)``(System.DateTime.Now,orderId,false,System.Decimal(0))
        ctx.SubmitUpdates()
        subOrder

    let getSubOrder subOrderId (ctx:DbContext) =
        log.Debug(sprintf "%s %d" "getSubOrder" subOrderId)
        ctx.Public.Suborder |> Seq.find (fun (x:SubOrder) -> x.Suborderid = subOrderId)

    let setAdjustmentOfSubOrder subOrderId adjustment (ctx:DbContext) =
        log.Debug(sprintf "%s %d %.2f" "setAdjustemtOfSubOrder" subOrderId adjustment)
        let subOrder = getSubOrder subOrderId ctx
        subOrder.Subtotaladjustment <- adjustment
        ctx.SubmitUpdates()

    let setPercentAdjustmentOfSubOrder subOrderId percentadjustment (ctx:DbContext) =
        log.Debug(sprintf "%s %d %.2f" "setPercentAdjustemtOfSubOrder" subOrderId percentadjustment)
        let subOrder = getSubOrder subOrderId ctx
        subOrder.Subtotalpercentadjustment <- percentadjustment
        ctx.SubmitUpdates()

    let getPaymentItemsOfSubOrder subOrderId (ctx:DbContext) =
        log.Debug(sprintf "%s %d" "getPaymentItmsOfSubOrder" subOrderId)
        let subOrder = getSubOrder subOrderId ctx
        let paymentItems = subOrder.``public.paymentitem by suborderid``
        paymentItems |> Seq.toList

    let getPaymentItemDetailsOfSubOrder subOrderId (ctx:DbContext) =
        log.Debug(sprintf "%s %d" "getPaymentItemDetailsOfSubOrder" subOrderId)
        query { 
            for item in ctx.Public.Paymentitemdetails do
            where (item.Suborderid = subOrderId)
            select item
        } |> Seq.toList

    let getPaymentItemsOfOrder orderId (ctx:DbContext) =
        log.Debug(sprintf "%s %d" "getPaymentItmsOfubOrder" orderId)
        let order = getOrder orderId ctx
        let paymentItems = order.``public.paymentitem by orderid``
        paymentItems |> Seq.toList

    let getPaymentItemDetailsOfOrder orderId (ctx:DbContext) =
        log.Debug(sprintf "%s %d" "getPaymentItemDetailsOfOrder" orderId)
        query { 
            for item in ctx.Public.Paymentitemdetails do
            where (item.Orderid = orderId)
            select item
        } |> Seq.toList 

    let removePaymentItem paymentItemId (ctx:DbContext) =
        log.Debug(sprintf "%s %d\n" "removePaymentItem" paymentItemId)
        let paymentItem = ctx.Public.Paymentitem |> Seq.tryFind (fun x -> x.Paymentid=paymentItemId)
        match paymentItem with
            | Some thePaymentItem  -> thePaymentItem.Delete(); ctx.SubmitUpdates()
            | _ -> ()

    let getOrderItemDetailsOfSubOrder (subOrderId:int) (ctx:DbContext) =
        log.Debug(sprintf "%s %d" "getSubOrderItems" subOrderId)
        query {
            for item in ctx.Public.Orderitemdetails do 
                where (item.Isinsasuborder = true && item.Suborderid = subOrderId) 
                select item
            
        } 

    let getSubOrdersOfOrderById (orderid:int) (ctx: DbContext) =
        log.Debug(sprintf "%s %d" "getSubOrdersOfOrderById" orderid)
        let order = getOrder orderid ctx
        order.``public.suborder by orderid`` |> Seq.toList |> List.sortBy (fun (x:SubOrder) -> x.Creationtime)

    let getOrderItemsOfSubOrder subOrderId (ctx: DbContext) =
        query {
            for orderItem in ctx.Public.Orderitems do
            where (orderItem.Suborderid = subOrderId && orderItem.Isinsasuborder = true)
            select orderItem
        } |> Seq.toList

    let getDoneOrderDetails (ctx: DbContext) : Orderdetail list =
        log.Debug("getDoneOrderDetails")
        query {
            for order in ctx.Public.Orderdetails do
                where (order.Ongoing = false && order.Archived = false) //  && order.Closingtime.)
                sortBy order.Closingtime
        } |> Seq.toList

    let updateTotalOfOrder orderId (ctx: DbContext) =
        log.Debug(sprintf "%s %d" "updateTotalOfOrder" orderId)
        let orderItems = getOrderItemsOfOrder orderId ctx

        let prices = orderItems |> List.map (fun (x:OrderItem) -> x.Price*((decimal)x.Quantity))

        let price = prices |> (List.fold (+) ((decimal)0))

        let order = getOrder orderId ctx
        order.Total <- price

        let _ = match (order.Adjustispercentage,order.Adjustisplain) with
                | (true,_) -> 
                    let _ = order.Adjustedtotal <- price + (price*(order.Percentagevariataion/(decimal)100))
                    ()
                | (_,true) -> 
                    let _ = order.Adjustedtotal <- price + order.Plaintotalvariation
                    ()
                | _ -> 
                    let _ = order.Adjustedtotal <- price
                    ()

        ctx.SubmitUpdates() 

    let forceDeleteSubOrder (subOrderId:int) (ctx: DbContext) =
        log.Debug(sprintf "%s %d" "forceDeleteSubOrder" subOrderId)
        let subOrder = getSubOrder subOrderId ctx

        // let connectedPaymentItems = subOrder.``public.paymentitem by suborderid``

        // let connectedInvoices = subOrder.``public.invoices by suborderid``
        // let _ = connectedInvoices |> Seq.iter (fun (x:Invoice) -> x.Delete() )

        // let _ = connectedPaymentItems |> Seq.iter (fun (x:PaymentItem) -> x.Delete())

        let orderItems = getOrderItemsOfSubOrder subOrderId ctx
        let _ = orderItems |> List.iter(fun (x:OrderItem) -> x.Suborderid <- 0; x.Isinsasuborder <- false) // null
        ctx.SubmitUpdates()
        subOrder.Delete()
        ctx.SubmitUpdates()

    let isSubOrderPaid subOrderId (ctx: DbContext) =
        log.Debug(sprintf "%s %d" "isSubOrderPayed" subOrderId)
        let subOrder = getSubOrder subOrderId ctx
        subOrder.Payed



module Users =
    let validateUser (username, password) (ctx : DbContext) : UsersView option =
        log.Debug(sprintf "%s %s" "validateUser" "username")
        query {
            for user in ctx.Public.Usersview do
                where (user.Username = username && user.Password = password && user.Enabled)
                select user
        } |> Seq.tryHead

    let getUser id (ctx: DbContext) =
        log.Debug(sprintf "%s %d" "getUser" id)
        ctx.Public.Users |> Seq.find (fun (x:User) -> x.Userid = id)  

    let getAllUsers (ctx: DbContext): User list =
        log.Debug("getAllUsers")
        ctx.Public.Users |> Seq.toList
    let getAllUsersView (ctx: DbContext): UsersView list =
        log.Debug("getAllUsersView")
        ctx.Public.Usersview |> Seq.toList

    let getOrdinaryUsersView (ctx:DbContext)  =
        log.Debug("getOrdinaryUsersView")
        ctx.Public.Usersview |> Seq.filter (fun (x:UsersView) -> not x.Istemporary ) |> Seq.toList

    let getTemporaryUsersView (ctx:DbContext) =
        log.Debug("getTemporaryUsersView")
        ctx.Public.Usersview |> Seq.filter (fun (x:UsersView) -> x.Istemporary ) |> Seq.sortByDescending (fun (x:UsersView) -> x.Creationtime) |>  Seq.toList

let tryGetIngredientByName ingredientName (ctx:DbContext) =
    query {
        for course in ctx.Public.Ingredient do 
        where (course.Name = ingredientName)
        select course
    } |> Seq.tryHead

let setEnablerStatesForUser states userId (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "setEnablerStatesForUser" userId)
    let _ = List.iter (fun (x:int) -> (ctx.Public.Waiteractionablestates.Create(x,userId) |> ignore))  states
    ctx.SubmitUpdates()

let safeDeleteSubOrder (subOrderId:int) (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "safeDeleteSubOrder" subOrderId)
    let subOrder = Orders.getSubOrder subOrderId ctx

    // let connectedInvoices = subOrder.``public.invoices by suborderid``
    // let connectedPaymentItems = subOrder.``public.paymentitem by suborderid``

    // let _ = connectedInvoices |> Seq.iter (fun (x:Invoice) -> x.Delete() )
    // let _ = connectedPaymentItems |> Seq.iter (fun (x:PaymentItem) -> x.Delete())

    ctx.SubmitUpdates()

    match subOrder.Payed with
    | false ->
        let orderItems = Orders.getOrderItemsOfSubOrder subOrderId ctx
        let _ = orderItems |> List.iter(fun (x:OrderItem) -> x.Suborderid <- 0; x.Isinsasuborder <- false) // null
        ctx.SubmitUpdates()
        subOrder.Delete()
        ctx.SubmitUpdates()
    | _ -> ()

let getIngredientPricesDetails id (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getIngredientPricesDetails" id)
    query {
        for item in ctx.Public.Ingredientpricedetails do
        where (item.Ingredientid = id)
        select item
    } |> Seq.toList

let listOfEnabledStatesForWaiter userId (ctx: DbContext) =
     log.Debug(sprintf "%s %d" "listOfEnabledStatesForWaiter" userId)
     ctx.Public.Waiteractionablestates  |> 
     Seq.filter (fun (x:WaiterActionableState) -> x.Userid = userId) |> 
     Seq.map (fun (x:WaiterActionableState) -> x.Stateid) |> Seq.toList

let getObserverRoleStatusCategory (ctx: DbContext) =
    ctx.Public.Observersrolestatuscategories |> Seq.toList

let getCategoryOfIngredients categoryid (ctx: DbContext) =
    log.Debug (sprintf "%s %d" "getCategoryOfIngredients" categoryid)
    ctx.Public.Ingredientcategory  |> Seq.find (fun (x:IngredientCategory) -> x.Ingredientcategoryid = categoryid)

let getEnablerRoleStatusCategory (ctx: DbContext) =
    log.Debug("getEnablerRoleStatusCategory")
    ctx.Public.Enablersrolestatuscategories |> Seq.toList

let getActionableDefaultStatesForWaiter (ctx: DbContext) =
    log.Debug("getActionableDefaultStatesForWaiter")
    ctx.Public.Defaultactionablestates |> Seq.toList

let getDefaultActionableStatesForTempUser (ctx: DbContext) =
    ctx.Public.TempUserDefaultActionableStates |> Seq.toList

let getActionableStatesForSpecificWaiter id (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getActionableStatesForSpecificWaiter" id)
    ctx.Public.Waiteractionablestates |> Seq.filter (fun (x:WaiterActionableState) -> x.Userid = id) |> Seq.toList

let getAllIngredientCategories (ctx:DbContext) =
    log.Debug("getAllIngredientCategories")
    ctx.Public.Ingredientcategory |> Seq.sortBy (fun (x:IngredientCategory) -> x.Name) |>  Seq.toList

let tryGetIngredientCourse courseId ingredientId (ctx:DbContext) =
    log.Debug(sprintf "%s %d %d" "tryGetIngredientCourse" courseId ingredientId)
    query {
        for item in ctx.Public.Ingredientcourse do
            where (item.Courseid = courseId && item.Ingredientid = ingredientId)
            select item
    } |> Seq.tryHead

let getIngredientsOfACourse courseId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getIngredientsOfACourse" courseId)
    ctx.Public.Ingredientofcourses |> Seq.filter (fun (x:IngredientOfCourse) -> x.Courseid = courseId) |> Seq.toList

let getVisibleIngredientCategories (ctx: DbContext) =
    ctx.Public.Ingredientcategory |> Seq.sortBy (fun (x:IngredientCategory) -> x.Name) |>  
        Seq.filter (fun (x:IngredientCategory) -> x.Visibility) |> Seq.toList

let getAllVisibleIngredientCategories (ctx:DbContext) =
    log.Debug(sprintf "getAllVisibleIngredientCategories")
    ctx.Public.Ingredientcategory |> Seq.filter (fun (x:IngredientCategory) -> x.Visibility) |> Seq.toList

let getAllIngredients (ctx:DbContext) =
    log.Debug("getAllIngredients")    
    ctx.Public.Ingredient |> Seq.toList

let getallIngredientsOfACategory idCategory (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getallIngredientsOfACategory" idCategory)
    query {
        for ingredient in ctx.Public.Ingredient do
            where (ingredient.Ingredientcategoryid = idCategory) 
            sortBy ingredient.Name
            select ingredient
    } |> Seq.toList

let getAllRoles (ctx:DbContext) : Role list =
    log.Debug("getAllroles")
    ctx.Public.Roles |> Seq.filter (fun (x:Role) -> x.Rolename <> "admin" && x.Rolename <> "powerUser" && x.Rolename <> "temporary") |> Seq.toList

let createPrinter name (ctx:DbContext) =
    log.Debug("createPrinter")
    let alreadyExists = ctx.Public.Printers |> Seq.tryFind (fun (x:Printer) -> x.Name = name)
    match alreadyExists with
    | None -> let _ = ctx.Public.Printers.``Create(name)``(name) 
              ctx.SubmitUpdates()
    | _ -> ()

let getPrinters (ctx: DbContext) =
    log.Debug("getPrinters")
    ctx.Public.Printers |> Seq.toList 

let getPrintersForReceipts (ctx:DbContext) =
    log.Debug("getPrintersForReceipts")
    query {
        for printer in ctx.Public.Printers do
        join printerForReceipts in ctx.Public.Printerforreceiptandinvoice on (printer.Printerid = printerForReceipts.Printerid)
        where (printerForReceipts.Printreceipt = true)
        select printer
    } |> Seq.toList

let getPrinter printerId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getPrinter" printerId)
    ctx.Public.Printers |> Seq.find (fun (x:Printer) -> x.Printerid = printerId) 

let getPrinterForCourseCategoryAssociation printerId (ctx:DbContext) =
    log.Debug("getPrinterForCourseCategoryAssociation")
    query {
        for printerAssociation in ctx.Public.Printerforcategory do
        where (printerAssociation.Printerid = printerId)
        select printerAssociation
    } |> Seq.toList

let getPrinterForReceiptAndInvoice printerId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getPrinterForReceiptAndInvoice" printerId)
    query {
        for printerAssociation in ctx.Public.Printerforreceiptandinvoice do
        where (printerAssociation.Printerid = printerId)
        select printerAssociation
    } |> Seq.toList
    
let safeRemovePrinter printerId (ctx:DbContext) =
    log.Debug("safeRemovePrinter")
    let printer = getPrinter printerId ctx
    // let printerCourseCatAssociation = getPrinterForCourseCategoryAssociation printerId ctx
    // let printerReceiptAndInvoiceAssociation = getPrinterForReceiptAndInvoice printerId ctx
    // printerCourseCatAssociation |> Seq.iter (fun x -> x.Delete())
    // printerReceiptAndInvoiceAssociation |> Seq.iter (fun x -> x.Delete())
    ctx.SubmitUpdates()
    printer.Delete()
    ctx.SubmitUpdates()



let getAllCustomers (ctx:DbContext) =
    ctx.Public.Customerdata |> Seq.toList

let tryFindCustomerDataByName name (ctx:DbContext) =
    ctx.Public.Customerdata |> Seq.tryFind (fun (x:CustomerData) -> x.Name = name)
    
let tryFindCustomerDataByData data (ctx:DbContext) =
    ctx.Public.Customerdata |> Seq.tryFind (fun (x:CustomerData) -> x.Data = data)

let tryUpdateCustomerData customerId companyName customerData (ctx:DbContext) =
    log.Debug("tryUpdateCustomerData")

    let company = ctx.Public.Customerdata |> Seq.tryFind (fun (x:CustomerData) -> x.Customerdataid = customerId)
    let (existing1,existing2) = 
        (ctx.Public.Customerdata |> Seq.tryFind (fun (x:CustomerData) -> x.Customerdataid <> customerId && x.Data = customerData),
         ctx.Public.Customerdata |> Seq.tryFind (fun (x:CustomerData) -> x.Customerdataid <> customerId && x.Name = companyName))

    match (existing1,existing2) with
    | (None,None) ->
         match company with 
            | Some theCompany -> theCompany.Name <- companyName
                                 theCompany.Data <- customerData
                                 ctx.SubmitUpdates()
            | None -> ()
    |  _ -> ()

let createCustmomerData (companyName: string) (customerData:string) (ctx:DbContext) =
    log.Debug("createCustomerData")
    let newCustomerData = ctx.Public.Customerdata.``Create(data, name)``(customerData,companyName)
    ctx.SubmitUpdates()
    newCustomerData

let createInvoiceBySubOrderIdWithNoCustomerId subOrderId invoiceText invoiceNumber (ctx:DbContext) =
    log.Debug("createInvoiceBySubOrderIdWithNoCustomerId")
    let now = System.DateTime.Now
    let newInvoice = ctx.Public.Invoices.``Create(data, date, invoicenumber)``(invoiceText,now,invoiceNumber)
    let _ = newInvoice.Suborderid <- subOrderId
    ctx.SubmitUpdates()

let createInvoiceByOrderIdWithNoCustomerId orderId invoiceText invoiceNumber (ctx:DbContext) =
    log.Debug("createInvoiceWithNoCustomerId")
    let now = System.DateTime.Now
    let newInvoice = ctx.Public.Invoices.``Create(data, date, invoicenumber)``(invoiceText,now,invoiceNumber)
    let _ = newInvoice.Orderid <- orderId
    ctx.SubmitUpdates()

let createInvoiceBysubOrderIdAndCustomerId subOrderId customerDataId invoiceText invoiceNumber (ctx:DbContext) =
    log.Debug("createInvoiceWithCustomerId")
    let now = System.DateTime.Now
    let newInvoice = ctx.Public.Invoices.``Create(data, date, invoicenumber)``(invoiceText,now,invoiceNumber)
    let _ = newInvoice.Customerdataid <- customerDataId
    let _ = newInvoice.Suborderid <- subOrderId
    ctx.SubmitUpdates()

let createInvoiceByOrderIdAndCustomerId orderId customerDataId invoiceText invoiceNumber (ctx:DbContext) =
    log.Debug("createInvoiceByOrderIdAndCustomerId")
    let now = System.DateTime.Now
    let newInvoice = ctx.Public.Invoices.``Create(data, date, invoicenumber)``(invoiceText,now,invoiceNumber)
    let _ = newInvoice.Customerdataid <- customerDataId
    let _ = newInvoice.Orderid <- orderId
    ctx.SubmitUpdates()

let safeRemovePrinters (ctx:DbContext) =
    log.Debug("safeRemovePrinters")
    // ctx.Public.Printerforcategory |> Seq.iter (fun (x:PrinterForCourseCategory) -> x.Delete())
    // ctx.SubmitUpdates()

    // ctx.Public.Printerforreceiptandinvoice |> Seq.iter (fun (x:PrinterForReceiptAndInvoice) -> x.Delete())
    // ctx.SubmitUpdates()

    ctx.Public.Printers |> Seq.iter (fun (x:Printer) -> x.Delete())
    ctx.SubmitUpdates()

let getOrderedListOfOrdinaryStates (ctx: DbContext): State list =
    log.Debug("getOrderedListOfOrdinaryStates")
    let rec rearrangeInOrder (myStates:State list) (iter:State list) indexOfFirst =
        match myStates with
        | [] -> iter
        | T -> 
            let first = T |> List.find (fun (x:State) -> x.Stateid = indexOfFirst) 
            let tail = T |> List.filter (fun (x:State) -> x.Stateid <> indexOfFirst) 
            let next = first.Nextstateid
            rearrangeInOrder tail (iter@[first]) next

    let states = States.getOrdinaryStates ctx
    let first = List.find (fun (x:State) -> x.Isinitial) states
    rearrangeInOrder states [] first.Stateid

let createWaiterActionableState (stateId:int) (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "createWaiterActionableState" stateId)
    let _ = ctx.Public.Defaultactionablestates.Create(stateId)
    ctx.SubmitUpdates()

let createTempUserActionableState (stateId:int) (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "createTempUserActionableState" stateId )
    let _ = ctx.Public.TempUserDefaultActionableStates.Create(stateId)
    ctx.SubmitUpdates()

let createSpecificWaiterAcionableState (userId:int) (stateId:int) (ctx:DbContext) =
    log.Debug(sprintf "%s %d %d" "createSpecificWaiterAcionableState" userId stateId)
    let _ = ctx.Public.Waiteractionablestates.Create(stateId,userId) 
    ctx.SubmitUpdates()

let deleteWaiterActionableState (stateId:int) (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "deleteWaiterActionableState" stateId)
    let existing = ctx.Public.Defaultactionablestates |> Seq.find (fun (x:DefaultActionableState) -> x.Stateid = stateId)
    existing.Delete()
    ctx.SubmitUpdates()

let deleteDefaultTempUserActionableState (stateId:int) (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "deleteDefaultTempUserActionableState" stateId)
    let existing = ctx.Public.TempUserDefaultActionableStates |> Seq.find (fun (x:TempUserDefaultActionableStates) -> x.Stateid = stateId)
    existing.Delete()
    ctx.SubmitUpdates()

let deleteSpecificWaiterAcionableState (userId:int ) (stateId:int) (ctx: DbContext) =
    log.Debug(sprintf "%s %d %d" "deleteSpecificWaiterAcionableState" userId stateId)
    let existing = ctx.Public.Waiteractionablestates |> Seq.find (fun (x:WaiterActionableState) -> (x.Stateid = stateId && x.Userid = userId))
    existing.Delete()
    ctx.SubmitUpdates()

let tryCreateRoleStateCategoryObserveMap (roleId:int) (stateId:int) (categoryId:int) (ctx: DbContext) =
        log.Debug(sprintf "%s %d %d %d" "tryCreateRoleStateCategoryObserveMap" roleId stateId categoryId)
        let existing = ctx.Public.Observers |> Seq.tryFind (fun (x:Observer) -> (x.Roleid = roleId && x.Stateid = stateId && x.Categoryid = categoryId))
        match existing with
        | Some _ -> None
        | None -> 
            let createdObserver = ctx.Public.Observers.Create(categoryId,roleId,stateId) 
            ctx.SubmitUpdates()
            Some createdObserver

let tryCreateRoleStateCategoryObserverByStateName (roleId:int) stateName (categoryId: int) (ctx:DbContext) =
    log.Debug(sprintf "%s %d %s" "tryCreateRoleStateCategoryObserverByStateName" roleId stateName)
    let state = States.tryGetStateByName stateName ctx
    match state with
    | Some theState -> tryCreateRoleStateCategoryObserveMap roleId theState.Stateid categoryId ctx
    | _ -> None
    
let tryCreateRoleStateCategoryEnablerMap (roleId:int) (stateId:int) (categoryId:int) (ctx: DbContext) =
        log.Debug(sprintf "%s %d %d %d" "tryCreateRoleStateCategoryEnablerMap" roleId stateId categoryId)
        let existing = ctx.Public.Enablers |> Seq.tryFind  (fun (x:Enabler) -> (x.Roleid = roleId && x.Stateid = stateId && x.Categoryid = categoryId))
        match existing with
        | Some _ -> None
        | None ->
            let createdEnabler = ctx.Public.Enablers.Create(categoryId,roleId,stateId) 
            ctx.SubmitUpdates()
            Some createdEnabler

let tryRemoveRoleStateCategoryEnablerMap (roleId:int) (stateId:int) (categoryId:int) (ctx: DbContext) =
        log.Debug(sprintf "%s %d %d %d" "tryRemoveRoleStateCategoryEnablerMap" roleId stateId categoryId)
        let enabler = ctx.Public.Enablers |> Seq.tryFind  (fun (x:Enabler) -> (x.Roleid = roleId && x.Stateid = stateId && x.Categoryid = categoryId))
        match enabler with
        | Some theEnabler -> 
            theEnabler.Delete()
            ctx.SubmitUpdates()
        | None -> ()

let tryRemoveRoleStateCategoryObserverMap (roleId:int) (stateId:int) (categoryId:int) (ctx: DbContext) =
        log.Debug(sprintf "%s %d %d %d" "tryRemoveRoleStateCategoryObserverMap" roleId stateId categoryId)
        let observer = ctx.Public.Observers |> Seq.tryFind  (fun (x:Observer) -> (x.Roleid = roleId && x.Stateid = stateId && x.Categoryid = categoryId))
        match observer with
        | Some theObserver -> 
            theObserver.Delete()
            ctx.SubmitUpdates()
        | None -> ()

let tryRemoveRoleStateCategoryEnablerByStateName (roleId:int) stateName (categoryId:int) (ctx: DbContext) =
    log.Debug(sprintf "%s %d %s %d" "tryRemoveRoleStateCategoryEnablerByStateName" roleId stateName categoryId)
    let state = States.tryGetStateByName stateName ctx
    match state with 
    | Some theState -> tryRemoveRoleStateCategoryEnablerMap roleId theState.Stateid categoryId ctx
    | _ -> ()

let tryRemoveRoleStateCategoryObserverByStateName (roleId:int) stateName (categoryId:int) (ctx: DbContext) =
    let state = States.tryGetStateByName stateName ctx
    match state with 
    | Some theState -> tryRemoveRoleStateCategoryObserverMap roleId theState.Stateid categoryId ctx
    | _ -> ()

let tryCreateRoleStateCategoryEnablerByStateName (roleId:int) stateName (categoryId: int) (ctx:DbContext) =
    let state = States.tryGetStateByName stateName ctx
    match state with
    | Some theState -> tryCreateRoleStateCategoryEnablerMap roleId theState.Stateid categoryId ctx
    | _ -> None

let getRoles (ctx: DbContext): Role list =
    ctx.Public.Roles |> Seq.toList

let createRole (name:string) (comment:string option) (ctx: DbContext) =
    let role = ctx.Public.Roles.Create(name)
    let theComment = match comment with Some X -> X | _ -> ""
    role.Comment<- theComment
    ctx.SubmitUpdates()


let getAllIngredientsOfACategoryByPage idCategory pageNumber (ctx: DbContext) =
    log.Debug(sprintf "%s %d %d" "getAllIngredientsOfACategoryByPage" idCategory pageNumber)
    let startIndex = pageNumber * Globals.NUM_DB_ITEMS_IN_A_PAGE
    let upperIndex = startIndex + Globals.NUM_DB_ITEMS_IN_A_PAGE 
    let allIngredientsOfCategory = 
        query {
            for ingredient in ctx.Public.Ingredient do
                where (idCategory = ingredient.Ingredientcategoryid)
                sortBy ingredient.Name
                skip (pageNumber*Globals.NUM_DB_ITEMS_IN_A_PAGE)
                take (upperIndex - startIndex)
                select ingredient
        } |> Seq.toList
    allIngredientsOfCategory 

let getAllIngredientsOfACategoryByPageWithNameSearch idCategory pageNumber (nameSearch: string) (ctx: DbContext) =
    log.Debug(sprintf "%s %d %d %s" "getAllIngredientsOfACategoryByPageWithNameSearch" idCategory pageNumber nameSearch )
    let resultSearch =
        query {
            for ingredient in ctx.Public.Ingredient do
                where (idCategory = ingredient.Ingredientcategoryid && ingredient.Name.ToLower().Contains(nameSearch.ToLower()))
                sortBy ingredient.Name
                select ingredient
        } |> Seq.toArray
    let startIndex = pageNumber * Globals.NUM_DB_ITEMS_IN_A_PAGE
    let upperIndex = min (resultSearch.Length - 1) (startIndex + Globals.NUM_DB_ITEMS_IN_A_PAGE - 1)
    if (startIndex < resultSearch.Length) then [startIndex .. upperIndex] |> List.map (fun i -> resultSearch.[i])
        else []
                
let tableIsAlreadyInAnOpenOrder table (ctx:DbContext) =
    let ordersOfTable =  query {
        for order in ctx.Public.Orders do
            where (order.Archived = false && order.Voided = false && order.Table = table)
            select order
    }
    Seq.length ordersOfTable >0
let createOrderByUser table person (userid:int) (ctx:DbContext) =
    log.Debug(sprintf "%s %s %d" "createOrderByUser" table userid)
    let order = ctx.Public.Orders.Create(false,person,System.DateTime.Now,table,userid)
    ctx.SubmitUpdates()
    order
let latestInvoiceNumber (ctx:DbContext) =
    log.Debug("latestInvoiceNumber")
    query {
        for invoice in ctx.Public.Invoices do
            sortByDescending invoice.Invoicenumber
            select invoice.Invoicenumber
    } |> Seq.tryHead

let getOrderItemsByOrderAndCourse(orderid,courseid) (ctx:DbContext): OrderItem option=
    log.Debug(sprintf "%s %d %d" "getOrderItemsByOrderAndCourse" orderid courseid)
    query {
        for orderItem in ctx.Public.Orderitems  do
            where (orderItem.Orderid = orderid && orderItem.Courseid = courseid)
            select orderItem
    } |> Seq.tryHead

let tryFindUserById userId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "tryFindUserById" userId)
    query {
        for user in ctx.Public.Users do
            where (user.Userid = userId)
            select user
    } |> Seq.tryHead

let getUserById userId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getUserById" userId)
    ctx.Public.Users |> Seq.find (fun (x:User) -> x.Userid = userId)

let tryGetPrinterForReceiptAndInvoice printerId (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "tryGetPrinterForReceiptAndInvoice" printerId)
    ctx.Public.Printerforreceiptandinvoice |> Seq.tryFind (fun (x:PrinterForReceiptAndInvoice) -> x.Printerid = printerId)

let tryFindUserView userId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "tryFindUserView" userId)
    query {
        for user in ctx.Public.Usersview do
            where (user.Userid = userId)
            select user
    } |> Seq.tryHead

let getCategoryOforderItem id (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getCategoryOforderItem" id)
    query {
        for category in ctx.Public.Coursecategories do
            where (category.Categoryid = id)
            select category
    } |> Seq.tryHead


let getFirstNAvailableOutGroupIdentifier  n (ctx:DbContext) =
    let usedIdentifiers = ctx.Public.Orderoutgroup |> Seq.map (fun x -> x.Groupidentifier) |> Seq.toList
    let firstNAvailbales = [for i in 1 .. 42 do if (not (List.contains i usedIdentifiers) ) then yield i] |> List.take n
    firstNAvailbales

let tryGetOutGroupByOrderIdAndGroupIdentifier orderId groupOut (ctx:DbContext) =
    log.Debug(sprintf "%s %d %d" "tryGetOutGroupByOrderIdAndGroupIdentifier" orderId groupOut)
    ctx.Public.Orderoutgroup |> Seq.tryFind (fun (x:OrderOutGroup) -> (x.Orderid = orderId && x.Groupidentifier =  groupOut))

let createOrGetOutGroup orderId groupOut (ctx:DbContext) =
    log.Debug(sprintf "%s %d %d" "createOrGetOutGroup" orderId groupOut)
    let existingGroup = tryGetOutGroupByOrderIdAndGroupIdentifier orderId groupOut ctx
    let newGroup = 
        match existingGroup with 
        | None -> ctx.Public.Orderoutgroup.``Create(groupidentifier, orderid, printcount)``(groupOut,orderId,0)
        | _ -> existingGroup.Value
    ctx.SubmitUpdates()
    newGroup

let createPlainUnitaryOrderItemById  orderId courseId subGroupIdOption (ctx:DbContext) =
    let finalState = States.getFinalState ctx
    let course = Courses.getCourse courseId ctx
    let price = course.Price
    let now = System.DateTime.Now
    let groupDbItem = createOrGetOutGroup orderId 99 ctx
    let _ = groupDbItem.Printcount <- 1
    let orderItem = ctx.Public.Orderitems.``Create(courseid, hasbeenrejected, ordergroupid, orderid, printcount, quantity, startingtime, stateid)`` (courseId,false,groupDbItem.Ordergroupid,orderId,1,1,now,finalState.Stateid)
    orderItem.Price <- price
    let _ = match subGroupIdOption with
                | Some N -> orderItem.Isinsasuborder <- true; orderItem.Suborderid <- N
                | _ -> ()

    ctx.SubmitUpdates()

let getTheOrderItemById orderItemId (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getTheOrderItemById" orderItemId)
    ctx.Public.Orderitems |> Seq.find(fun (x:OrderItem) -> x.Orderitemid = orderItemId )

let unBoundDifferentSubGroupsOfOrderItemsByIs (ids: int list)  (ctx: DbContext)=
    let orderItemsInSuborders = ids |> List.map (fun x -> getTheOrderItemById x ctx) |> List.filter (fun x -> x.Isinsasuborder)
    let differentSubOrdersIds = orderItemsInSuborders |> List.map (fun x -> x.Suborderid) |> Set.ofList
    match (Set.count differentSubOrdersIds) with
            | N when (N > 1) -> differentSubOrdersIds |>   Set.iter (fun x -> Orders.forceDeleteSubOrder x ctx); None
            | 1 -> Set.toList differentSubOrdersIds |> List.tryHead
            | _ -> None
let createOrderItemByCourseName courseName orderid quantity comment price (groupOut: decimal) (ctx: DbContext) =
    log.Debug(sprintf "%s %s %d %.2f" "createOrderItemByCourseName" courseName orderid  price  )
    let course = match Courses.tryFindCourseByName courseName ctx with
        | Some X -> X
        | None -> failwith ("unexisting course with name "+ courseName)
    let initState = States.getInitState ctx
    let now = System.DateTime.Now

    let groupDbItem = createOrGetOutGroup orderid ((int)groupOut) ctx
    let orderItem = ctx.Public.Orderitems.``Create(courseid, hasbeenrejected, ordergroupid, orderid, printcount, quantity, startingtime, stateid)`` (course.Courseid,false,groupDbItem.Ordergroupid,orderid,0,quantity,now,initState.Stateid)

    let _ = orderItem.Comment <- match comment with | Some theComment -> theComment | _  -> ""

    orderItem.Price<-price
    ctx.SubmitUpdates()
    let _ = ctx.Public.Orderitemstates.Create(orderItem.Orderitemid,now,initState.Stateid)
    ctx.SubmitUpdates()
    orderItem

let cloneOrderItemStatesWithNewOrderItemId orderItemId (orderitemstate:OrderItemState) (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "cloneOrderItemStatesWithNewOrderItemId" orderItemId)
    let stateId = orderitemstate.Stateid
    let startingTime = orderitemstate.Startingtime
    let clonedState = ctx.Public.Orderitemstates.``Create(orderitemid, startingtime, stateid)``(orderItemId,startingTime,stateId)
    ctx.SubmitUpdates()
    clonedState

let createUnitaryNakedOrderItemByOrderId courseId orderId comment price outGroupId (ctx: DbContext) =
    log.Debug(sprintf "%s %d %d" "createUnitaryNakedOrderItemByOrderId" courseId orderId )
    let now = System.DateTime.Now
    let finalState = States.getFinalState ctx
    let orderItem = ctx.Public.Orderitems.``Create(courseid, hasbeenrejected, ordergroupid, orderid, printcount, quantity, startingtime, stateid)`` (courseId,false,outGroupId,orderId,1,1,now,finalState.Stateid)
    let _ = orderItem.Comment <- comment
    let _ = orderItem.Price <- price
    ctx.SubmitUpdates()
    orderItem

let getDistinctOutGroupsOfOrder orderId (ctx:DbContext) =
    log.Debug(sprintf "%s %d " "getDistinctOutGroupsOfOrder" orderId)
    let order = Orders.getOrder orderId ctx
    let orderItems = order.``public.orderitems by orderid``
    let outGroups = orderItems |> Seq.map (fun (x:OrderItem ) -> x.Ordergroupid) |> Set.ofSeq
    outGroups

let createRejectedOrderItem orderItemId courseId cause (ctx:DbContext) =
    log.Debug(sprintf "%s %d %d %s" "createRejectedOrderItem" orderItemId courseId cause )
    ctx.Public.Rejectedorderitems.Create(cause,courseId,orderItemId,System.DateTime.Now)

let bindOrderItemToSubOrder orderItemId subOrderId (ctx: DbContext) =
    log.Debug(sprintf "%s %d %d" "bindOrderItemToSubOrder" orderItemId subOrderId)
    let orderItem = getTheOrderItemById orderItemId ctx
    orderItem.Isinsasuborder <- true
    orderItem.Suborderid <- subOrderId
    let subOrder = Orders.getSubOrder subOrderId ctx
    subOrder.Subtotal <- subOrder.Subtotal + (orderItem.Price * (decimal)orderItem.Quantity)
    ctx.SubmitUpdates()

let updatePasswordOfUser idUser passwordHash (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "updatePasswordOfUser" idUser)
    let user = getUserById idUser ctx
    user.Password<- passwordHash
    ctx.SubmitUpdates()

let updateOrderItem (orderItemId, courseid, quantity, comment) (ctx: DbContext) =
     log.Debug(sprintf "%s %d %d" "updateOrderItem" orderItemId courseid)
     let tryOrderItem = Orders.getOrderItemById orderItemId ctx
     
     let thecomment  = match comment with | Some c -> c | None -> ""
     match tryOrderItem with
     | Some orderItem -> orderItem.Quantity<-quantity; orderItem.Courseid<-courseid; orderItem.Comment<-thecomment;  ctx.SubmitUpdates()
     | None -> ()

let updateUserStatus id status canVoidorders canManageAllOrders canChangeThePrices canManageAllCourses (ctx: DbContext) =
    let userView = tryFindUserView id ctx
    let user = getUserById id ctx
    match userView with 
    | Some aUser ->
        if (aUser.Rolename <> "admin") then user.Enabled <- status else ()
        user.Canvoidorders<- canVoidorders
        user.Canmanageallorders <- canManageAllOrders
        user.Canchangetheprice <- canChangeThePrices
        user.Canmanagecourses <- canManageAllCourses
    | _ -> ()
    ctx.SubmitUpdates()

let getAllVisibleIngredients (ctx: DbContext) =
    log.Debug("getAllVisibleIngredients")
    ctx.Public.Ingredient |> Seq.filter (fun (x:Ingredient) -> x.Visibility) |> Seq.toList

let getAllVisibleIngredientsOfCourse (ctx: DbContext) =
    log.Debug("getAllVisibleIngredientsOfCourse")
    ctx.Public.Ingredientofcourses |> Seq.filter (fun (x:IngredientOfCourse) -> x.Visibility) |> Seq.toList

let getAllVisibleIngredientDetails (ctx: DbContext) =
    log.Debug("getAllVisibleIngredientDetails")
    ctx.Public.Ingredientdetails |> Seq.filter (fun (x:IngredientDetail) -> x.Visibility && x.Categoryvisibility) |> Seq.toList |> List.sortBy (fun (x:IngredientDetail) -> x.Ingredientname)

let getVisibleIngredientsOfCourseOfACategory categoryId (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getVisibleIngredientsOfCourseOfACategory" categoryId)
    ctx.Public.Ingredientofcourses |> Seq.filter (fun (x:IngredientOfCourse) -> x.Ingredientcategoryid = categoryId && x.Visibility ) |> Seq.toList

let getVisibleIngredientsDetailOfACategory categoryId (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getVisibleIngredientsDetailOfACategory" categoryId)
    ctx.Public.Ingredientdetails |> Seq.filter (fun (x:IngredientDetail) -> x.Ingredientcategoryid = categoryId && x.Visibility && x.Categoryvisibility)  |> Seq.sortBy (fun (x:IngredientDetail) -> x.Ingredientname) |>  Seq.toList

let getVisibleIngredientsOfACategory categoryId (ctx: DbContext) =
     log.Debug(sprintf "%s %d" "getVisibleIngredientsOfACategory" categoryId)
     query {
        for ingredient in ctx.Public.Ingredient do
            where (ingredient.Ingredientcategoryid = categoryId && ingredient.Visibility)
            sortBy ingredient.Name
            select ingredient
     } |> Seq.toList

let addIngredientToCourse ingredientId courseId (quantity: decimal option) (ctx: DbContext) =
    log.Debug(sprintf "%s %d %d" "addIngredientToCourse" ingredientId courseId)
    let ingredientCourseAssociation = ctx.Public.Ingredientcourse.Create(courseId,ingredientId)
    match quantity with
    | Some theQuantity -> ingredientCourseAssociation.Quantity <- theQuantity
    | _ -> ()
    ctx.SubmitUpdates()

let getStatesNextStatesPairs (ctx: DbContext) =
    log.Debug(sprintf "%s" "getStatesNextStatesPairs" )
    let states = ctx.Public.States |> Seq.toList
    let nextStates = List.map (fun (x:State) -> (x.Stateid,  if (x.Isfinal) then x else (States.getState x.Nextstateid ctx) )) states
    nextStates

let getMapOfNextStates (ctx: DbContext) =
    log.Debug(sprintf "%s" "getMapOfNextStates")
    States.getOrdinaryStates ctx |>  List.map (fun (x:State) -> (x.Stateid,  if (x.Isfinal) then x else (States.getState x.Nextstateid ctx) ))  |> Map.ofSeq

let getMapOfStates (ctx: DbContext) =
    log.Debug("getMapOfStates")
    (States.getAllStates ctx) |> List.map (fun (x:State) -> (x.Stateid,x)) |> Map.ofSeq

let setAdjstedTotalOfOrder orderId total (ctx:DbContext) =
    log.Debug(sprintf "%s %d %.2f" "setAdjstedTotalOfOrder" orderId total)
    let order =Orders.getOrder orderId ctx
    order.Adjustedtotal <- total
    ctx.SubmitUpdates()

let setTotalOfOrder orderId total (ctx: DbContext) =
    log.Debug(sprintf "%s %d %.2f" "setTotalOfOrder" orderId total )
    let order =Orders.getOrder orderId ctx
    order.Total <- total
    ctx.SubmitUpdates()

let getOrderItemsOfOrderById orderId (ctx: DbContext): OrderItem list =
    log.Debug(sprintf "%s %d" "getOrderItemsOfOrderById" orderId) 
    query {
        for orderItem in ctx.Public.Orderitems do
            where (orderItem.Orderid = orderId)
            select orderItem
        }  |> Seq.toList

let getIngredientIdsOfACourse courseId (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getIngredientIdsOfACourse" courseId )
    query {
        for ingredientCourse in ctx.Public.Ingredientcourse do
            where (ingredientCourse.Courseid = courseId)
            select ingredientCourse.Ingredientid
    } |> Seq.toList

let tryGetOrderById orderId (ctx:DbContext ): Order option = 
    log.Debug(sprintf "%s %d" "tryGetOrderById" orderId)
    query {
        for order in ctx.Public.Orders do
            where (order.Orderid = orderId)
            select order
    } |> Seq.tryHead

let getOrderItemDetailsOfSubOrder subOrderId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getOrderItemDetailsOfSubOrder" subOrderId )
    query {
        for orderItemDetail in ctx.Public.Orderitemdetails do
            where (orderItemDetail.Isinsasuborder = true &&  orderItemDetail.Suborderid = subOrderId)
            sortBy (orderItemDetail.Startingtime)
            select orderItemDetail
    } |> Seq.toList

let getOrderItemDetailsOfOrder orderId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getOrderItemDetailsOfOrder" orderId )
    query {
        for orderItemDetail in ctx.Public.Orderitemdetails do
            where (orderItemDetail.Orderid = orderId)
            sortBy (orderItemDetail.Startingtime)
            select orderItemDetail
    } |> Seq.toList

let getOrderItemDetailsOfSubOrderThatAreNotInInitState subOrderId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getOrderItemDetailsOfSubOrderThatAreNotInInitState" subOrderId)
    let initState = States.getInitState ctx

    query {
        for orderItemDetail in ctx.Public.Orderitemdetails do
            where (orderItemDetail.Suborderid = subOrderId && orderItemDetail.Stateid <> initState.Stateid)
            sortBy (orderItemDetail.Startingtime)
            select orderItemDetail
    } |> Seq.toList

let getOrderItemDetailOfOrder (order:Order) (ctx: DbContext) =
    log.Debug(sprintf "%s" "getOrderItemDetailOfOrder")
    query {
        for orderItem in ctx.Public.Orderitemdetails do
            where (orderItem.Orderid = order.Orderid)
            sortBy (orderItem.Startingtime)
            select orderItem
        } |> Seq.toList

let getOrderItemDetailOfOrderById orderId (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getOrderItemDetailOfOrderById" orderId)
    query {
        for orderItem in ctx.Public.Orderitemdetails do
            where (orderItem.Orderid = orderId)
            sortBy (orderItem.Startingtime)
            select orderItem
        } |> Seq.toList

let getUnpaidOrderItemDetailOfOrderById orderId (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getUnpaidOrderItemDetailOfOrderById" orderId)
    getOrderItemDetailOfOrderById orderId ctx |> List.filter (fun (x:OrderItemDetails) -> (not x.Payed))

let getOrderItemDetailOfOrderThatArenotInInitState orderId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getOrderItemDetailOfOrderThatArenotInInitState" orderId)
    let initState = States.getInitState ctx
    query {
        for orderItem in ctx.Public.Orderitemdetails do
            where (orderItem.Orderid = orderId && orderItem.Stateid <> initState.Stateid)
            sortBy (orderItem.Startingtime)
            select orderItem
        } |> Seq.toList

let getOrderDetailsOfGroupDetail (oderoutGroup:OrderOutGroupDetail) (ctx: DbContext) =
    log.Debug(sprintf "%s %d " "getOrderDetailsOfGroupDetail" oderoutGroup.Ordergroupid)
    query {
        for orderItemDetail in ctx.Public.Orderitemdetails do
        where (orderItemDetail.Ordergroupid = oderoutGroup.Ordergroupid)
        select orderItemDetail
    } |> Seq.toList

let getOrderItemsDetailOfOrderByOutGroup outGroupId (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getOrderItemsDetailOfOrderByOutGroup" outGroupId)
    query {
        for orderItem in ctx.Public.Orderitemdetails do
            where (orderItem.Ordergroupid = outGroupId)
            sortBy (orderItem.Startingtime)
            select orderItem
        } |> Seq.toList

let getOrderItemsOfOrderByOutGroup outGroupId (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getOrderItemsOfOrderByOutGroup" outGroupId)
    query {
        for orderItem in ctx.Public.Orderitems do
            where (orderItem.Ordergroupid = outGroupId)
            sortBy (orderItem.Startingtime)
            select orderItem
        } |> Seq.toList

let someItemsOfAnOutGroupAreInInitialState (outGroup:OrderOutGroup) (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "someItemsOfAnOutGroupAreInInitialState" outGroup.Ordergroupid)    
    let initialStateId = (States.getInitState ctx).Stateid
    let orderItems = outGroup.``public.orderitems by ordergroupid``
    orderItems |> Seq.exists (fun (x:OrderItem) -> x.Stateid = initialStateId)

let getOutGroup outGroupId (ctx: DbContext) =
    log.Debug(sprintf "%s %d " "getOutGroup" outGroupId)
    ctx.Public.Orderoutgroup |> Seq.find (fun (x:OrderOutGroup) -> x.Ordergroupid = outGroupId)

let getOutGroupDetail outGroupId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getOutGroupDetail" outGroupId)
    ctx.Public.Orderoutgroupdetails |> Seq.find (fun (x:OrderOutGroupDetail) -> x.Ordergroupid = outGroupId)

let getOutGroupsOfOrder orderId (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getOutGroupsOfOrder" orderId)
    query {
        for outGroup in ctx.Public.Orderoutgroup do
            where (outGroup.Orderid = orderId)
            sortBy (outGroup.Groupidentifier)
            select outGroup
    } |> Seq.toList

let getOutGroupOfOrdeHavingSomeItemsInInitialState orderId (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getOutGroupOfOrdeHavingSomeItemsInInitialState" orderId)
    getOutGroupsOfOrder orderId ctx |> List.filter (fun (x:OrderOutGroup) -> someItemsOfAnOutGroupAreInInitialState x ctx)

let getAlreadyPrintedOutGroupsOfOrderItems orderId (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getAlreadyPrintedOutGroupsOfOrderItems" orderId)
    getOutGroupsOfOrder orderId ctx |> List.filter (fun (x:OrderOutGroup) -> (x.Printcount > 1))


let getOrderItemDetailOfOrderDetail (order: Orderdetail) (ctx: DbContext): OrderItemDetails list  =
    log.Debug(sprintf "%s %d" "getOrderItemDetailOfOrderDetail" order.Orderid)
    query {
        for orderItem in ctx.Public.Orderitemdetails do
            where (orderItem.Orderid = order.Orderid)
            sortBy (orderItem.Startingtime)
            select orderItem
        } |> Seq.toList

let getOrderItemDetailOfOrderDetailThatAreNotInInitState (order: Orderdetail) (ctx: DbContext): OrderItemDetails list  =
    log.Debug(sprintf "%s %d" "getOrderItemDetailOfOrderDetailThatAreNotInInitState" order.Orderid)
    let initState = States.getInitState ctx
    let initStateid = initState.Stateid
    query {
        for orderItem in ctx.Public.Orderitemdetails do
            where (orderItem.Orderid = order.Orderid && orderItem.Stateid <> initStateid)
            sortBy (orderItem.Startingtime)
            select orderItem
        } |> Seq.toList

let getOrderItemDetailOfOrderDetailThatAreNotInInitStateByNonEmptyOrderDetail (order: NonArchivedOrderDetail) (ctx: DbContext): OrderItemDetails list  =
    log.Debug(sprintf "%s %d" "getOrderItemDetailOfOrderDetailThatAreNotInInitState" order.Orderid)
    let initState = States.getInitState ctx
    let initStateid = initState.Stateid
    query {
        for orderItem in ctx.Public.Orderitemdetails do
            where (orderItem.Orderid = order.Orderid && orderItem.Stateid <> initStateid)
            sortBy (orderItem.Startingtime)
            select orderItem
        } |> Seq.toList

let newUser canChangeThePrices canSeeAllOrders username password roleid canManageAllCourses  (ctx : DbContext) =
    log.Debug(sprintf "%s %s" "newUser" username)
    let user = ctx.Public.Users.``Create(canchangetheprice, canmanageallorders, canmanagecourses, enabled, istemporary, password, role, username)``(canChangeThePrices,canSeeAllOrders,canManageAllCourses,true,false,password,roleid,username)
    ctx.SubmitUpdates()
    user

let tryFindIngredientCatgoryByName name (ctx: DbContext): IngredientCategory option =
    query {
        for ingredientCategory in  ctx.Public.Ingredientcategory do
            where (ingredientCategory.Name = name)
            select ingredientCategory
    } |> Seq.tryHead

let getIngredientCatgory id  (ctx: DbContext) =
    ctx.Public.Ingredientcategory |> Seq.find (fun (x:IngredientCategory) -> x.Ingredientcategoryid = id) 

let newCategory (categoryName:string) (visibility:bool) (abstractness:bool) (ctx: DbContext) =
    let category = ctx.Public.Coursecategories.``Create(abstract, name)``(abstractness,categoryName)
    category.Visibile<-visibility
    ctx.SubmitUpdates()

let tryFindUserByName username (ctx : DbContext) : User option = 
   log.Debug("tryFindUserByName")
   query {
       for user in ctx.Public.Users do
       where (user.Username = username)
       select user
   } |> Seq.tryHead

let tryFindUsersViewByName username (ctx : DbContext) : UsersView option = 
   log.Debug("tryFindUsersViewByName")
   query {
       for user in ctx.Public.Usersview do
       where (user.Username = username)
       select user
   } |> Seq.tryHead

let getAllOrderItemsOfOrder orderId (ctx:DbContext) =
    log.Debug("getAllOrderItemsOfOrder")
    query {
        for orderItem in ctx.Public.Orderitems do
            where (orderItem.Orderid = orderId)
            select orderItem
    } |> Seq.toList

let getIngredientPrice id (ctx:DbContext) =
    log.Debug("getIngredientprice")
    ctx.Public.Ingredientprice |> Seq.find (fun (x:IngredientPrice) -> x.Ingredientpriceid = id )

let getAllOrderItemDetailsOfOrder orderId (ctx:DbContext) =
    log.Debug("getAllOrderItemDetailsOfOrder")
    query {
        for orderItem in ctx.Public.Orderitemdetails do
            where (orderItem.Orderid = orderId)
            select orderItem
    } |> Seq.toList

let userEnabledByRoletoChangeStateOfThisOrderItem (orderItemDetail:OrderItemDetails)  (userRoleId: int) (ctx: DbContext) =
        log.Debug("userEnabledByRoletoChangeStateOfThisOrderItem")
        query  { 
            for enabler in ctx.Public.Enablers do
                where (orderItemDetail.Categoryid = enabler.Categoryid &&
                    orderItemDetail.Stateid = enabler.Stateid && 
                    userRoleId = enabler.Roleid)
                select enabler
        } |> Seq.isEmpty |> not

let tryGetNextState (stateId: int) (ctx:DbContext): State option =
    log.Debug("tryGetNextState")
    let currentState = ctx.Public.States |> Seq.filter (fun (x:State) -> x.Stateid = stateId) |> Seq.tryHead
    match currentState with
    | Some theCurrentState -> 
        let nextState = ctx.Public.States |> Seq.filter (fun (x:State) -> x.Stateid = theCurrentState.Nextstateid) |> Seq.tryHead
        nextState
    | None -> None

let getNextStateId (stateId: int) (ctx:DbContext) =
    log.Debug("getNextStateId")
    let currentState = ctx.Public.States |> Seq.find (fun (x:State) -> x.Stateid = stateId)
    currentState.Nextstateid

let tryGetOrderItemById(orderItemId: int) (ctx: DbContext) =
    log.Debug("tryGetOrderItemById")
    query {
        for orderItem  in ctx.Public.Orderitems do
            where (orderItem.Orderitemid = orderItemId)
            select orderItem
    } |> Seq.tryHead

let tryGetOrderItemDetailById orderItemId (ctx: DbContext) =
    log.Debug("tryGetOrderItemDetailById")
    query {
        for orderItemDetail  in ctx.Public.Orderitemdetails do
            where (orderItemDetail.Orderitemid = orderItemId)
            select orderItemDetail
    } |> Seq.tryHead

let createIngredientDecrementsOfOrderItem (orderItem: OrderItem) preparatorId (ctx: DbContext)  =
    log.Debug(sprintf "%s %d %d" "createIngredientDecrementsOfOrderItem" orderItem.Orderitemid preparatorId)
    let course = Courses.getCourse orderItem.Courseid ctx
    let plainIngredientIds = course.``public.ingredientcourse by courseid`` |> 
        Seq.map (fun (x:IngredientCourse) -> x.Ingredientid)

    let presumedIngredientQuantities = course.``public.ingredientcourse by courseid`` |>
        Seq.map (fun (x:IngredientCourse) -> (x.Ingredientid,(x.Quantity*(decimal)orderItem.Quantity))) |> Map.ofSeq

    let variations = orderItem.``public.variations by orderitemid``

    let idSOfVariationsExtraIngredients = variations |>
        (Seq.filter (fun (x:Variation) -> (not (Seq.contains x.Ingredientid plainIngredientIds)))) |>
        (Seq.map (fun (x:Variation) -> x.Variationsid))

    let idsOfVariationsWithout = variations |> 
        (Seq.filter (fun (x:Variation) -> x.Tipovariazione = Globals.SENZA)) |> 
        (Seq.map (fun (x:Variation ) -> x.Ingredientid))
        |> Seq.toList
        
    let idsOfVariationsLess = variations |> 
        Seq.filter (fun (x:Variation) -> x.Tipovariazione = Globals.POCO) |> 
        (Seq.map (fun (x:Variation ) -> x.Ingredientid))
        |> Seq.toList

    let idsOfVariationsMore = variations |> 
        Seq.filter (fun (x:Variation) -> x.Tipovariazione = Globals.MOLTO) |> 
        (Seq.map (fun (x:Variation ) -> x.Ingredientid))
        |> Seq.toList

    let idsOfVariationsAddLittle = variations |> 
        Seq.filter (fun (x:Variation) -> x.Tipovariazione = Globals.AGGIUNGIPOCO) |> 
        Seq.map (fun (x:Variation ) -> x.Ingredientid)
        |> Seq.toList

    let idsOfVariationsAddNormal = variations |>  
        Seq.filter (fun (x:Variation) -> x.Tipovariazione = Globals.AGGIUNGINORMALE) |> 
        Seq.map (fun (x:Variation ) -> x.Ingredientid)
        |> Seq.toList

    let idsOfVariationsAddMuch = variations |>  
        Seq.filter (fun (x:Variation) -> x.Tipovariazione = Globals.AGGIUNGIMOLTO) |> 
        Seq.map (fun (x:Variation ) -> x.Ingredientid)
        |> Seq.toList

    let plainNetIngredientsByTheBook = plainIngredientIds |> 
        Seq.filter (fun x -> (not (Seq.contains x idsOfVariationsWithout))) |> 
        Seq.filter (fun x -> (not (Seq.contains x idsOfVariationsLess))) |> 
        (Seq.filter (fun x -> (not (Seq.contains x idsOfVariationsMore))))
        |> Seq.toList

    let plainIngredientWithVariations = (plainNetIngredientsByTheBook |> 
        (List.map (fun x -> (x,Globals.NORMAL)))) @ (idsOfVariationsLess |> 
        List.map (fun id -> (id,Globals.POCO))) @ (idsOfVariationsMore |> 
        List.map (fun id -> (id,Globals.MOLTO)))

    let allTogetherIngredientVariations = plainIngredientWithVariations @ 
        (idsOfVariationsAddLittle |> List.map (fun id -> (id,Globals.AGGIUNGIPOCO))) @
        (idsOfVariationsAddNormal |> List.map (fun id -> (id,Globals.AGGIUNGINORMALE))) @
        (idsOfVariationsAddMuch |> List.map (fun id -> (id,Globals.AGGIUNGIMOLTO))) 

    let setOfVariations = allTogetherIngredientVariations |> Set.ofList

    let now = System.DateTime.Now

    let _ = 
         setOfVariations |> Set.iter (fun (ingredientid,variationType) -> (
              let tryPresumedNormalQuantity = Map.tryFind ingredientid presumedIngredientQuantities
              match tryPresumedNormalQuantity  with
                | Some theNormalQuantity when theNormalQuantity> 0.0M  -> 
                    let ing = ctx.Public.Ingredientdecrement.``Create(ingredientid, orderitemid, preparatorid, registrationtime, typeofdecrement)``(ingredientid, orderItem.Orderitemid, preparatorId, now, variationType)
                    ing.Presumednormalquantity <- theNormalQuantity
                    ctx.SubmitUpdates()
                | _ -> ()
             )
         )

    let _ = 
         let extras = variations |> Seq.filter (fun (x:Variation) -> (Seq.contains x.Variationsid idSOfVariationsExtraIngredients))
         extras |> Seq.iter (fun (x:Variation) ->
            match (x.Tipovariazione,x.Plailnumvariation) with
                | (Globals.UNITARY_MEASUSERE,X) when (X <> 0) -> 
                    let ing = ctx.Public.Ingredientdecrement.``Create(ingredientid, orderitemid, preparatorid, registrationtime, typeofdecrement)``(x.Ingredientid, orderItem.Orderitemid, preparatorId, now, x.Tipovariazione)
                    ing.Presumednormalquantity <- (decimal)X
                    ctx.SubmitUpdates()
                | (Globals.PER_PREZZO_INGREDIENTE,_) ->
                    let ingPrice = getIngredientPrice x.Ingredientpriceid ctx 
                    let ing = ctx.Public.Ingredientdecrement.``Create(ingredientid, orderitemid, preparatorid, registrationtime, typeofdecrement)``(x.Ingredientid, orderItem.Orderitemid, preparatorId, now, x.Tipovariazione)
                    ing.Presumednormalquantity <- (decimal) ingPrice.Quantity
                    ctx.SubmitUpdates()

                    ()
                | _ -> ()
                )
    ()

let createCloneOfRejectedOrderitem newOrderItemId (rejectedOrderItemToBeCloned: RejectedOrderItems) (ctx:DbContext)=
    log.Debug("createCloneOfRejectedOrderitem")
    let cloneOfRejectedOrderItem = ctx.Public.Rejectedorderitems.``Create(cause, courseid, orderitemid, timeofrejection)`` (rejectedOrderItemToBeCloned.Cause,rejectedOrderItemToBeCloned.Courseid,newOrderItemId,rejectedOrderItemToBeCloned.Timeofrejection)
    ctx.SubmitUpdates()
    cloneOfRejectedOrderItem

let createClonedIngredientDecrement newOrderItemId (splitFactor: decimal) (ingredientDecrementToBeCloned: IngredientDecrement ) (ctx:DbContext) =
    log.Debug(sprintf "%s %d " "createClonedIngredientDecrement" newOrderItemId)
    let splittedPresumedNormalQuantity = ingredientDecrementToBeCloned.Presumednormalquantity / splitFactor
    let splittedRecordedQuantity = ingredientDecrementToBeCloned.Recordedquantity / splitFactor

    let clonedDecrement = ctx.Public.Ingredientdecrement.``Create(ingredientid, orderitemid, preparatorid, registrationtime, typeofdecrement)`` (ingredientDecrementToBeCloned.Ingredientid,newOrderItemId,ingredientDecrementToBeCloned.Preparatorid,ingredientDecrementToBeCloned.Registrationtime,ingredientDecrementToBeCloned.Typeofdecrement)
    let _ = clonedDecrement.Presumednormalquantity <- splittedPresumedNormalQuantity
    let _ = clonedDecrement.Recordedquantity <- splittedRecordedQuantity
    ctx.SubmitUpdates()
    clonedDecrement

let createClonedVariationOfOrderItem newOrderItemid ingredientId kindOfVariation (ctx:DbContext) =
    log.Debug("createClonedVariationOfOrderItem")
    let clonedVariation = ctx.Public.Variations.``Create(ingredientid, orderitemid, tipovariazione)``(ingredientId,newOrderItemid,kindOfVariation)
    clonedVariation

let createClonedRejectedOrderItem newOrderItemid (rejecteOrderItemToBeCloned:RejectedOrderItems) (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "createClonedRejectedOrderItem" newOrderItemid )
    let clonedRejectedOrderItem = ctx.Public.Rejectedorderitems.``Create(cause, courseid, orderitemid, timeofrejection)``(rejecteOrderItemToBeCloned.Cause,rejecteOrderItemToBeCloned.Courseid,newOrderItemid,rejecteOrderItemToBeCloned.Timeofrejection)
    clonedRejectedOrderItem

let createClonedOrderItemState newOrderItemId (orderItemState:OrderItemState) (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "createClonedOrderItemState" newOrderItemId)
    let clonedOrderItemState = ctx.Public.Orderitemstates.``Create(orderitemid, startingtime, stateid)`` (newOrderItemId,orderItemState.Startingtime,orderItemState.Stateid )
    clonedOrderItemState

let getDecrementViewById (ingredientDecrementId:int) (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getDecrementViewById" ingredientDecrementId)
    ctx.Public.Ingredientdecrementview |> Seq.filter (fun ingredientDecrementView -> ingredientDecrementView.Ingredientdecrementid = ingredientDecrementId) |> Seq.head

let getIngredientById id (ctx:DbContext) =
    log.Debug("getIngredientById")
    ctx.Public.Ingredient |> Seq.filter (fun (x:Ingredient) -> x.Ingredientid = id) |> Seq.head

let decrementIngredinetAvailabilityByAmount (ingredientDecrement:IngredientDecrement) (ctx:DbContext) =
    log.Debug("decrementIngredinetAvailabilityByAmount")
    let ingredient = getIngredientById (ingredientDecrement.Ingredientid) ctx
    ingredient.Availablequantity <- ingredient.Availablequantity - ingredientDecrement.Presumednormalquantity

let decreaseIngredientAvailabilityByAmount ingredientId amount (ctx:DbContext) =
    log.Debug("decreaseIngredientAvailabilityByAmount")
    let ingredient = getIngredientById ingredientId ctx
    ingredient.Availablequantity <- ingredient.Availablequantity - amount
    ctx.SubmitUpdates()

let getIngredientDecrementsByOrderItemId orderItemId  (ctx:DbContext) =
    log.Debug("getIngredientDecrementsByOrderItemId")
    ctx.Public.Ingredientdecrement |> Seq.filter (fun (x:IngredientDecrement) -> x.Orderitemid = orderItemId)

let updateAvailableQuantityOfIngredientsOfOrderItems (orderItem:OrderItem) (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "updateAvailableQuantityOfIngredientsOfOrderItems" orderItem.Orderitemid)
    let decrements = getIngredientDecrementsByOrderItemId orderItem.Orderitemid ctx
    match decrements |> Seq.length with
    | X when (X>0) -> 
        let decrementsViews = decrements |> Seq.map (fun decrement -> getDecrementViewById decrement.Ingredientdecrementid ctx) 
        let updatableDecrementsView = decrementsViews |> Seq.filter (fun (x:IngredientDecrementView) -> x.Updateavailabilityflag)
        updatableDecrementsView |> Seq.iter (fun (x:IngredientDecrementView) -> decreaseIngredientAvailabilityByAmount x.Ingredientid x.Presumednormalquantity ctx)
    | _ ->  ()

let isInitialState stateId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "isInitialState" stateId)
    let state = ctx.Public.States |> Seq.find (fun (x:State) -> x.Stateid = stateId)
    state.Isinitial

let isOrderItemHavingOrdersItemAtInitialState orderId (ctx:DbContext) =
    let orderItemAtInitialState = getAllOrderItemDetailsOfOrder orderId ctx |> 
        List.tryFind (fun (x:OrderItemDetails) -> isInitialState x.Stateid ctx)
    match orderItemAtInitialState with
    | Some _ -> true
    | _ -> false

let tryMoveOrderItemToNextState (orderItemId: int) userId (ctx: DbContext) =
    log.Debug(sprintf "%s %d %d" "tryMoveOrderItemToNextState" orderItemId userId)
    let orderItem = tryGetOrderItemById orderItemId ctx
    match orderItem with
    | Some theOrderItem ->
        let nextState = tryGetNextState theOrderItem.Stateid ctx
        match nextState with 
        | Some theNextState ->
            theOrderItem.Stateid <- theNextState.Stateid
            ctx.SubmitUpdates()
            let now = System.DateTime.Now

            let _ = ctx.Public.Orderitemstates.Create(orderItemId,now,theNextState.Stateid)
            let _ = if (theNextState.Isfinal) then theOrderItem.Closingtime<- now else ()

            let _ =
                log.Debug(sprintf "%s %s %b" "here is the next state " theNextState.Statusname theNextState.Creatingingredientdecrement)
                match theNextState.Creatingingredientdecrement with
                | true ->
                  log.Debug("is creating ingredient decrement")
                  createIngredientDecrementsOfOrderItem theOrderItem userId ctx
                  updateAvailableQuantityOfIngredientsOfOrderItems theOrderItem ctx
                | false -> ()


            ctx.SubmitUpdates()
        | None -> ()
    | None -> failwith (sprintf "%s %d " "unexisting order item with id " orderItemId)

let updateCourseCategory (courseCategory:CourseCategories) name visibility abstractness (ctx: DbContext) =
        log.Debug(sprintf "%s %d %s" "updateCourseCategory" courseCategory.Categoryid name)
        courseCategory.Name<-name
        courseCategory.Visibile<-visibility
        courseCategory.Abstract <- abstractness

        ctx.SubmitUpdates()

let voidOrder id (ctx: DbContext) =
    let order = tryGetOrderById id ctx
    match order with
     | Some x -> x.Voided <- true; ctx.SubmitUpdates()
     | _ -> () 

let getRole id (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "getrole" id)
    ctx.Public.Roles |> Seq.find (fun (x:Role) -> x.Roleid = id)

let getObserversCatgoryStateMappingForRole (roleid:int) (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getObserversCatgoryStateMappingForRole" roleid)
    query {
        for observer in ctx.Public.Observers do
        where (observer.Roleid = roleid)
        select observer
    } |> Seq.toList

let getEnablersCatgoryStateMappingForRole (roleid:int) (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getEnablersCatgoryStateMappingForRole" roleid)
    query {
        for enabler in ctx.Public.Enablers do
        where (enabler.Roleid = roleid)
        select enabler
    } |> Seq.toList

let getOrderItemDetailsOfAParticularState (stateId) (ctx: DbContext) =
    log.Debug(sprintf "%s %d" "getOrderItemDetailsOfAParticularState" stateId)
    query {
        for orderItem in ctx.Public.Orderitemdetails do
        where (orderItem.Stateid = stateId)
        select orderItem
    } |> Seq.toList

let getOrderItemDetailsOfAParticularStateAndAParticularCategory (stateId,categoryId) (ctx: DbContext) =
    log.Debug("getOrderItemDetailsOfAParticularStateAndAParticularCategory")
    query {
        for orderItem in ctx.Public.Orderitemdetails do
        where (orderItem.Stateid = stateId && orderItem.Categoryid = categoryId)
        select orderItem
    } |> Seq.toList

let tryGetObserverRoleCategorySatateMappingById  (id:int)  (ctx: DbContext) =
    log.Debug("tryGetObserverRoleCategorySatateMappingById")
    query {
        for observer in ctx.Public.Observers do
        where (observer.Observersid = id)
        select observer
    } |> Seq.tryHead

let tryGetEnablerRoleCategorySatateMappingById  (id:int)  (ctx: DbContext) =
    log.Debug("tryGetEnablerRoleCategorySatateMappingById")
    query {
        for enabler in ctx.Public.Enablers do
        where (enabler.Enablersid = id)
        select enabler
    } |> Seq.tryHead

let deleteEnabler id (ctx: DbContext) =
    log.Debug("deleteEnabler")
    let enabler = tryGetEnablerRoleCategorySatateMappingById id ctx
    match enabler with
    | Some theEnabler -> theEnabler.Delete(); ctx.SubmitUpdates()
    | None -> ()

let isUserAdmin userId (ctx: DbContext) =
    log.Debug(sprintf "%s %dd " "isUserAdmin" userId)
    let isAdmin = query {
            for user in ctx.Public.Users do
                join role in ctx.Public.Roles on (user.Role = role.Roleid)
                where (user.Userid = userId && role.Rolename = "admin")
            } 
    not (Seq.isEmpty isAdmin)

let tryGetEnablerByRoleAndCategory roleid categoryid (ctx: DbContext) =
    log.Debug("tryGetEnablerByRoleAndCategory")
    query {
        for enabler in ctx.Public.Enablers do
        where (enabler.Categoryid = categoryid && enabler.Roleid = roleid) 
        select enabler
    } |> Seq.tryHead

let deleteObserver id (ctx: DbContext) =
    log.Debug("deleteObserver")
    let observer = tryGetObserverRoleCategorySatateMappingById id ctx

    match observer with
    | Some theObserver -> 
         let enabler  = tryGetEnablerByRoleAndCategory theObserver.Roleid theObserver.Categoryid ctx
         match enabler with 
         | Some theEnabler -> theEnabler.Delete()
         | None -> ()
         theObserver.Delete()
         ctx.SubmitUpdates()
    | None ->  log.Error("error unknown id for observer")

let archiveOrder id (ctx:DbContext) =
    log.Debug("archiveOrder")
    let order = tryGetOrderById id ctx
    match order with
    | Some theOrder -> 
        theOrder.Archived <- true
        theOrder.Person <- "" 
        theOrder.Closingtime <- System.DateTime.Now
        ctx.SubmitUpdates()
    | _ -> ()

let isUserEnabledToSeeWholeOrders userId (ctx:DbContext) =
    log.Debug("isUserEnabledToSeeWholeOrders")
    let user = getUserById userId  ctx
    user.Canmanageallorders

let getLatestLogOrder (ctx: DbContext) =
    log.Debug("getLatestLogOrder")
    query {
        for order in ctx.Public.Archivedorderslogbuffer do
        sortByDescending order.Archivedtime
        select order
    } |> Seq.tryHead

let getNumbeOfAllIngredientsOfACategory idCategory (ctx: DbContext) =
    log.Debug("getNumbeOfAllIngredientsOfACategory")
    ctx.Public.Ingredient |> Seq.filter (fun (x:Ingredient) -> (x.Ingredientcategoryid = idCategory)) |> Seq.length

let getLatestVoidedOrder userId (ctx:DbContext) =
    log.Debug("getLatestVoidedOrder")
    query {
        for voidedOrder in ctx.Public.Voidedorderslogbuffer do
        where (voidedOrder.Userid = userId)
        sortByDescending voidedOrder.Voidedtime
        select voidedOrder
    } |> Seq.tryHead

let newIngredientCatgory name visibility description (ctx: DbContext) = 
    log.Debug("newIngredientCatgory")
    let newIngredient = ctx.Public.Ingredientcategory.Create(name, visibility)
    newIngredient.Description <- description
    ctx.SubmitUpdates()

let newIngredient name visibility description idCategory allergene 
    (availableQuantity:decimal) updateAvailabilityFlag checkAvailabilityFlag unitOfMeasure (ctx: DbContext) =
    log.Debug("newIngredient")

    let newIngredient = ctx.Public.Ingredient.``Create(allergen, checkavailabilityflag, ingredientcategoryid, name, unitmeasure, updateavailabilityflag, visibility)`` (allergene,checkAvailabilityFlag, idCategory,name,unitOfMeasure, updateAvailabilityFlag,visibility)

    newIngredient.Description <- description
    newIngredient.Availablequantity  <- availableQuantity

    ctx.SubmitUpdates()
    newIngredient

let tryGetIngredientCategory id (ctx: DbContext) =
    log.Debug("tryGetIngredientCategory")
    query {
        for category in ctx.Public.Ingredientcategory do
        where (category.Ingredientcategoryid = id)
        select category
    } |> Seq.tryHead

let tryGetIngredientById id (ctx: DbContext) =
    log.Debug("tryGetIngredientById")
    query {
        for ingredient in ctx.Public.Ingredient do
        where (ingredient.Ingredientid = id)
        select ingredient
    } |> Seq.tryHead

let findIngredientByName name (ctx:DbContext) =
    log.Debug("findIngredientByName")
    query {
        for ingredient in ctx.Public.Ingredient do
        where (ingredient.Name = name)
        select ingredient
    } |> Seq.tryHead

let addIngredientToCourseByName ingredientName courseId quantity ctx =
    log.Debug("addIngredientToCourseByName")
    let ingredient = findIngredientByName ingredientName ctx
    match ingredient with
    | Some theIngredient -> 
        addIngredientToCourse theIngredient.Ingredientid courseId quantity ctx
    | None -> ()

let getIngredientPrices ingredientId (ctx:DbContext) =
    log.Debug("getIngredientPrices")
    query {
        for item in ctx.Public.Ingredientprice do
        where (item.Ingredientid=ingredientId)
        select item
    } |> Seq.toList

let tryFindIngridientPriceOfQuantity ingredientId quantity (ctx: DbContext) =
    log.Debug("getIngredientPrices")
    ctx.Public.Ingredientprice |> Seq.tryFind (fun (x:IngredientPrice) -> (x.Ingredientid = ingredientId && x.Quantity = quantity))

let makeAllIngredientPriceNoDefaultAdd (ctx:DbContext) =
    log.Debug("makeAllIngredientPriceNoDefaultAdd")
    let _ = ctx.Public.Ingredientprice |> Seq.iter (fun (x:IngredientPrice) -> x.Isdefaultadd <- false) 
    ctx.SubmitUpdates()

let makeAllIngredientPriceNoDefaultSubtract (ctx:DbContext) =
    log.Debug("makeAllIngredientPriceNoDefaultSubtract")
    let _ = ctx.Public.Ingredientprice |> Seq.iter (fun (x:IngredientPrice) -> x.Isdefaultsubtract <- false) 
    ctx.SubmitUpdates()

let makeAllIngredientPriceOfIngredientNoDefaultAdd ingredientId (ctx:DbContext) =
    log.Debug("makeAllIngredientPriceNoDefaultAdd")
    let _ = ctx.Public.Ingredientprice |> Seq.filter(fun (x:IngredientPrice) -> x.Ingredientid = ingredientId) |> Seq.iter (fun (x:IngredientPrice) -> x.Isdefaultadd <- false) 
    ctx.SubmitUpdates()

let makeAllIngredientPriceOfIngredientNoDefaultSubtract ingredientId (ctx:DbContext) =
    log.Debug("makeAllIngredientPriceNoDefaultSubtract")
    let _ = ctx.Public.Ingredientprice |> Seq.filter(fun (x:IngredientPrice) -> x.Ingredientid = ingredientId ) |> Seq.iter (fun (x:IngredientPrice) -> x.Isdefaultsubtract <- false) 
    ctx.SubmitUpdates()

let tryCreateIngredientPrice addPrice ingredientId isDefaultAdd isDefaultSubtract quantity subtractPrice (ctx: DbContext) =
    log.Debug("tryCreateIngredientPrice")
    let alreadyExisting = tryFindIngridientPriceOfQuantity ingredientId quantity ctx
    match alreadyExisting with
    | Some x -> None
    | None -> 
        let _ = if (isDefaultAdd) then makeAllIngredientPriceOfIngredientNoDefaultAdd ingredientId ctx
        let _ = if (isDefaultSubtract) then makeAllIngredientPriceOfIngredientNoDefaultSubtract ingredientId ctx
        let newIngredientPrice =  ctx.Public.Ingredientprice.``Create(addprice, ingredientid, isdefaultadd, isdefaultsubtract, quantity, subtractprice)`` (addPrice,ingredientId,isDefaultAdd,isDefaultSubtract,quantity,subtractPrice)
        ctx.SubmitUpdates()
        Some newIngredientPrice

let deleteIngredientPrice (ingredientPrice:IngredientPrice) (ctx:DbContext) =
    log.Debug("deleteIngredientPrice")
    ingredientPrice.Delete()
    ctx.SubmitUpdates()

let safeDeleteVariation (variation:Variation) (ctx:DbContext) =
    log.Debug("safeDeleteVariation")
    variation.Delete()
    ctx.SubmitUpdates()

let safeDeleteIngredientPrice (ingredientPrice:IngredientPrice) (ctx:DbContext) =
    log.Debug("safeDeleteIngredientPrice")
    let variations = ingredientPrice.``public.variations by ingredientpriceid``
    let _  = variations |> Seq.iter  (fun (x:Variation) -> safeDeleteVariation x ctx)
    ingredientPrice.Delete()
    ctx.SubmitUpdates()

let getIngredientsOfCourse courseId (ctx:DbContext) =
    log.Debug("getIngredientsOfCourse")
    query {
        for ingredientOfCourse in ctx.Public.Ingredientofcourses do
        where (ingredientOfCourse.Courseid = courseId)
        select ingredientOfCourse
    } |> Seq.toList

let tryGetUnitaryIngredientVariationOfOrderItemAndIngredient orderItemId ingredientId (ctx:DbContext) =
    log.Debug("tryGetUnitaryIngredientVariationOfOrderItemAndIngredient")
    query {
        for variation in ctx.Public.Variations do
        join ingredient in ctx.Public.Ingredient on (variation.Ingredientid = ingredient.Ingredientid)
        where (variation.Ingredientid = ingredientId && variation.Orderitemid = orderItemId && ingredient.Unitmeasure = Globals.UNITARY_MEASUSERE)
        select variation
    } |> Seq.tryHead

let tryGetIngredientVariationOfOrderItemAndIngredient orderItemId ingredientId (ctx:DbContext) =
    log.Debug("tryGetIngredientVariationOfOrderItemAndIngredient")
    query {
        for variation in ctx.Public.Variations do
        where (variation.Ingredientid = ingredientId && variation.Orderitemid = orderItemId)
        select variation
    } |> Seq.tryHead
let addIncreaseIngredientVariation orderItemId ingredientId (ctx:DbContext) =
    log.Debug("addIncreaseIngredientVariation")
    let existingVariation = tryGetIngredientVariationOfOrderItemAndIngredient orderItemId ingredientId ctx
    let _ = match existingVariation with
            | Some theExistingVariation -> theExistingVariation.Delete(); ctx.SubmitUpdates()
            | _ -> ()

    let _ = ctx.Public.Variations.Create(ingredientId,orderItemId,Globals.MOLTO)
    ctx.SubmitUpdates()
let addAddNormalIngredientVariation orderItemId ingredientId (ctx:DbContext) =
    log.Debug("addAddNormalIngredientVariation")
    let existingVariation = tryGetIngredientVariationOfOrderItemAndIngredient orderItemId ingredientId ctx
    let _ = match existingVariation with   
        | Some theExistingVariation -> theExistingVariation.Delete(); ctx.SubmitUpdates()
        | _ -> ()
    let _ = ctx.Public.Variations.Create(ingredientId, orderItemId, Globals.AGGIUNGINORMALE)
    ctx.SubmitUpdates()
let addAddIngredientVariation orderItemId ingredientId quantity (ctx:DbContext) =
    log.Debug("addAddIngredientVariation")
    let existingVariation = tryGetIngredientVariationOfOrderItemAndIngredient orderItemId ingredientId ctx
    let _ = match existingVariation with   
        | Some theExistingVariation -> theExistingVariation.Delete(); ctx.SubmitUpdates()
        | _ -> ()
    let (newVariation:Variation) = ctx.Public.Variations.Create(ingredientId, orderItemId, quantity)
    let _ = match quantity with | Globals.UNITARY_MEASUSERE -> newVariation.Plailnumvariation <- 1 | _ -> ()    
    ctx.SubmitUpdates()
let addIngredientVariationByIngredientPriceRef orderItemId ingredientId ingredientPriceId (ctx:DbContext) =
    log.Debug("addIngredientVariationByIngredientPriceRef")
    let existingVariation = tryGetIngredientVariationOfOrderItemAndIngredient orderItemId ingredientId ctx
    let _ = match existingVariation with   
        | Some theExistingVariation -> theExistingVariation.Delete(); ctx.SubmitUpdates()
        | _ -> ()
    let (newVariation:Variation) = ctx.Public.Variations.Create(ingredientId,orderItemId,Globals.PER_PREZZO_INGREDIENTE)
    newVariation.Ingredientpriceid <- ingredientPriceId 
    ctx.SubmitUpdates()
let getFirstPriceVariationForIngredientAddVariatonFlaggedAsDefault ingredientId (ctx:DbContext) =
    log.Debug("getFirstPriceVariationForIngredientAddVariatonFlaggedAsDefault")
    let defaultAdd =
        query {
                for priceVariation in ctx.Public.Ingredientprice do
                where (priceVariation.Isdefaultadd && priceVariation.Ingredientid = ingredientId)
                select priceVariation
        } |> Seq.tryHead
    match defaultAdd with
    | Some X -> X.Addprice
    | None -> (decimal)0.0
    
let getFirstPriceVariationForIngredientSubtractVariatonFlaggedAsDefault ingredientId (ctx:DbContext) =
    log.Debug("getFirstPriceVariationForIngredientSubtractVariatonFlaggedAsDefault")
    let defaultSubtract =
        query {
                for priceVariation in ctx.Public.Ingredientprice do
                where (priceVariation.Isdefaultsubtract && priceVariation.Ingredientid = ingredientId)
                select priceVariation
        } |> Seq.tryHead
    match defaultSubtract with
    | Some X -> X.Subtractprice
    | None -> (decimal)0.0
let addAddIngredientVariationByName orderItemid ingredientName quantity (ctx:DbContext) =
    log.Debug("addAddIngredientVariationByName")
    let  mut = ref 1.1
    let ingredient = ctx.Public.Ingredient |> Seq.tryFind  (fun (x:Ingredient) -> x.Name = ingredientName)
    match ingredient with
    | Some theIngredient -> 
                            if (not (Double.TryParse(quantity,mut)) && not (quantity = Globals.UNITARY_MEASUSERE)) then 
                            (
                                let overWrittenQuantity = match theIngredient.Unitmeasure with 
                                    | (Globals.UNITARY_MEASUSERE) -> Globals.UNITARY_MEASUSERE 
                                    | _ -> quantity
                                addAddIngredientVariation orderItemid theIngredient.Ingredientid overWrittenQuantity (ctx:DbContext)
                            ) else
                            (
                                let ingredientPriceId = Int32.Parse(quantity)
                                addIngredientVariationByIngredientPriceRef orderItemid theIngredient.Ingredientid ingredientPriceId ctx
                            )

    | None -> ()
    ctx.SubmitUpdates()
let addDiminuishIngredientVariattion orderItemId ingredientId (ctx:DbContext) =
    log.Debug("addDiminuishIngredientVariattion")
    let existingVariation = tryGetIngredientVariationOfOrderItemAndIngredient orderItemId ingredientId ctx
    let _ = match existingVariation with
            | Some theExistingVariation -> theExistingVariation.Delete(); ctx.SubmitUpdates()
            | _ -> ()

    let _ = ctx.Public.Variations.Create(ingredientId,orderItemId,Globals.POCO)
    ctx.SubmitUpdates()
let tryGetVariation variationId (ctx: DbContext) =
    log.Debug("tryGetVariation")
    ctx.Public.Variations |> Seq.tryFind (fun (x:Variation) -> x.Variationsid = variationId)
let removeIngredientVariation variationId (ctx: DbContext) =
    log.Debug("removeIngredientVariation")
    let variation = tryGetVariation variationId ctx
    match variation with 
    | Some theVariation -> theVariation.Delete(); ctx.SubmitUpdates()
    | None -> ()

let removeEventuallyExistingVariation orderItemId ingredientId (ctx:DbContext) =
    log.Debug("removeEventuallyExistingVariation")
    let existingVariation = tryGetIngredientVariationOfOrderItemAndIngredient orderItemId ingredientId ctx
    match existingVariation with 
    | Some theExistingVariation -> theExistingVariation.Delete(); ctx.SubmitUpdates()
    | None -> ()

let addAddUnitaryIngredientVariationOrIncreaseByOne orderItemId ingredientId (ctx:DbContext) =
     log.Debug("addAddUnitaryIngredientVariationOrIncreaseByOne")
     let existingUnitaryVariation = tryGetUnitaryIngredientVariationOfOrderItemAndIngredient orderItemId ingredientId ctx 
     match existingUnitaryVariation with
     | Some theExistingUnitaryVariation -> 
        theExistingUnitaryVariation.Plailnumvariation <- theExistingUnitaryVariation.Plailnumvariation + 1
        ctx.SubmitUpdates()
     | None ->
        removeEventuallyExistingVariation orderItemId ingredientId ctx
        let newVariation = ctx.Public.Variations.``Create(ingredientid, orderitemid, tipovariazione)`` (ingredientId,orderItemId,Globals.UNITARY_MEASUSERE)
        newVariation.Plailnumvariation <- 1
        ctx.SubmitUpdates()
            
let getIdsOfNonUnitaryOrderItemsOfOrder orderId (ctx:DbContext) = 
    query {
        for orderItem in ctx.Public.Orderitems do
            where (orderItem.Orderid = orderId && orderItem.Quantity>1)
            select orderItem.Orderitemid
    } 

let addRemoveUnitaryIngredientVariationOrDecreaseByOne orderItemId ingredientId (ctx:DbContext) =
     log.Debug("addRemoveUnitaryIngredientVariationOrDecreaseByOne")
     let existingUnitaryVariation = tryGetUnitaryIngredientVariationOfOrderItemAndIngredient orderItemId ingredientId ctx 
     match existingUnitaryVariation with 
     | Some theExistingUnitaryVariation  -> 
        theExistingUnitaryVariation.Plailnumvariation <- theExistingUnitaryVariation.Plailnumvariation - 1
        ctx.SubmitUpdates()
     | None -> 
        removeEventuallyExistingVariation orderItemId ingredientId ctx
        let newVariation = ctx.Public.Variations.``Create(ingredientid, orderitemid, tipovariazione)`` (ingredientId,orderItemId,Globals.UNITARY_MEASUSERE)
        newVariation.Plailnumvariation <- -1
        ctx.SubmitUpdates()

let addRemoveIngredientVariation orderItemId ingrdientId (ctx:DbContext) =
    log.Debug("addRemoveIngredientVariation")
    let existingVariation = tryGetIngredientVariationOfOrderItemAndIngredient orderItemId ingrdientId ctx
    let _ = match existingVariation with
            | Some theExistingVariation -> theExistingVariation.Delete(); ctx.SubmitUpdates()
            | _ -> ()
    let _ = ctx.Public.Variations.Create(ingrdientId,orderItemId,Globals.SENZA)
    ctx.SubmitUpdates()

let getAllVariationDetailsOfOrderItem orderItemId (ctx:DbContext) =
    log.Debug("getAllVariationDetailsOfOrderItem")
    query {
        for variationDetail in ctx.Public.Variationdetails do
        where (variationDetail.Orderitemid = orderItemId ) 
        select variationDetail
    } |> Seq.toList

let getAllVariationsOfOrderItem orderItemId   (ctx:DbContext): Variation list =
    query {
        for variation in ctx.Public.Variations do
        where (variation.Orderitemid = orderItemId)
        select variation
    } |> Seq.toList

let SetOrderItemAsRejected orderItemId (ctx:DbContext) =
    let orderItem = getTheOrderItemById orderItemId ctx
    orderItem.Hasbeenrejected <- true;
    ctx.SubmitUpdates()

let removeAllVariationsOfOrderItem orderItemId (ctx:DbContext) =
    let variations = getAllVariationsOfOrderItem orderItemId ctx
    let _ = variations |> List.iter (fun (x:Variation) -> x.Delete())
    ctx.SubmitUpdates()

let updateOrderItemAndPriceByCourseName orderItemId courseName quantity comment newPrice (groupOut: decimal ) (ctx: DbContext) =
     let tryOrderItem = Orders.getOrderItemById orderItemId ctx
     let thecomment  = match comment with | Some c -> c | None -> ""

     let course = Courses.tryGetCourseByName courseName ctx

     match (tryOrderItem,course) with
     | (Some orderItem,Some theCourse) -> 
        let oldCourseId = orderItem.Courseid
        orderItem.Quantity<-quantity
        orderItem.Courseid<-theCourse.Courseid 
        orderItem.Comment<-thecomment 
        orderItem.Price <- newPrice

        let orderGroupOut = createOrGetOutGroup orderItem.Orderid ((int)groupOut) ctx

        orderItem.Ordergroupid <- (int)orderGroupOut.Ordergroupid
        orderItem.Hasbeenrejected <- false

        ctx.SubmitUpdates()
        let x = Orders.updateTotalOfOrder orderItem.Orderid ctx
        let _ = if (oldCourseId <> theCourse.Courseid) then (removeAllVariationsOfOrderItem orderItemId ctx)  else ()
        ()
     | _ -> ()

let getVariationDetailsOfOrderItem orderItemId (ctx: DbContext) =
    query {
        for variation in ctx.Public.Variationdetails do
        where (variation.Orderitemid = orderItemId)
        select variation
    } |> Seq.toList

let deleteAllVariationsOfOrderItem orderItemId (ctx:DbContext) =
    log.Debug("deleteAllVariationsOfOrderItem")
    let variations = getAllVariationsOfOrderItem orderItemId ctx
    List.iter (fun (x:Variation) -> x.Delete()) variations
    ctx.SubmitUpdates()

let deleteAllRejectionOfOrderItem orderItemId (ctx:DbContext) =
    log.Debug("deleteAllRejectionOfOrderItem")
    let rejectionOfOrderItems = ctx.Public.Rejectedorderitems |> Seq.filter(fun (x:RejectedOrderItems) -> x.Orderitemid = orderItemId)  
    Seq.iter (fun (x:RejectedOrderItems) -> x.Delete()) rejectionOfOrderItems
    ctx.SubmitUpdates()

let getOrderOutGroup groupId (ctx:DbContext) =
    ctx.Public.Orderoutgroup |> Seq.find (fun (x:OrderOutGroup) -> x.Ordergroupid = groupId)

let deleteOrderOutGroupByIdIfItIsEmpty groupId ctx =
    log.Debug(sprintf "%s %d" "deleteOrderOutGroupIfItIsEmpty" groupId)
    let group = getOrderOutGroup groupId ctx
    let orderItemsOfGroup = group.``public.orderitems by ordergroupid``
    let _ = if (Seq.isEmpty) orderItemsOfGroup then 
                group.Delete() 
                ctx.SubmitUpdates()
    ()

let deleteOrderOutGroupItIsEmpty (group:OrderOutGroup) (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "deleteOrderOutGroupIfItIsEmpty" group.Ordergroupid)
    let orderItemsOfGroup = group.``public.orderitems by ordergroupid``
    let _ = if (Seq.isEmpty) orderItemsOfGroup then 
                group.Delete() 
                ctx.SubmitUpdates()
    ()


let deleteAnyEmptyOrderOutGroupOfOrder orderId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "deleteAnyEmptyOrderOutGroupOfOrder" orderId)
    let order =Orders.getOrder orderId ctx
    let orderOutGroups = order.``public.orderoutgroup by orderid``
    let _ = orderOutGroups |> Seq.iter (fun (x:OrderOutGroup) -> deleteOrderOutGroupItIsEmpty x ctx)
    ()

let isOrderItemPayed (orderItem: OrderItem) (ctx:DbContext)  =
    match orderItem.Isinsasuborder with
        | false -> false 
        | true -> let theSubOrder = ctx.Public.Suborder |> Seq.find (fun (x:SubOrder) -> x.Suborderid = orderItem.Suborderid)
                  theSubOrder.Payed

let orderOutGroupsOfOrder (order: Order) (ctx:DbContext) =
    let groups = order.``public.orderoutgroup by orderid``
    groups

let movableOrderItems (orderItems: OrderItem list) (ctx:DbContext) =
    orderItems |> List.filter (fun (y:OrderItem) -> not (isOrderItemPayed y ctx))

let unBoundOrderItemFromAnySubGroupIfItIsNotPayed orderItemId  (ctx:DbContext) =
    let orderItem = getTheOrderItemById orderItemId ctx
    match isOrderItemPayed orderItem ctx with
        | false -> orderItem.Isinsasuborder <- false
                   ctx.SubmitUpdates()
        | true ->()

let tryMoveOrderItemToAnOutGroupOfAnotherOrder orderItemId outGroupId (ctx:DbContext) =
    let orderItem = getTheOrderItemById orderItemId ctx
    let outGroup = getOrderOutGroup outGroupId ctx
    let oriOutGroup = getOrderOutGroup orderItem.Ordergroupid ctx
    match isOrderItemPayed orderItem ctx with
        | true -> ()
        | false -> unBoundOrderItemFromAnySubGroupIfItIsNotPayed orderItemId ctx
                   orderItem.Ordergroupid <- outGroupId
                   orderItem.Orderid <- outGroup.Orderid
                   ctx.SubmitUpdates()
                   deleteOrderOutGroupItIsEmpty oriOutGroup ctx

let deleteOrderItem orderItemid (ctx:DbContext) =
    log.Debug("deleteOrderItem")
    let tryOrderItem = Orders.getOrderItemById orderItemid ctx
    match tryOrderItem with
    | Some orderItem -> 
        let orderOutGroupId = orderItem.Ordergroupid
        let _ = deleteAllVariationsOfOrderItem orderItemid ctx
        let _ = deleteAllRejectionOfOrderItem orderItemid ctx
        List.iter (fun (x:OrderItemState) -> x.Delete())  (orderItem.``public.orderitemstates by orderitemid``|> Seq.toList)
        ctx.SubmitUpdates()
        orderItem.Delete()
        ctx.SubmitUpdates()
        deleteOrderOutGroupByIdIfItIsEmpty orderOutGroupId ctx

    | None -> () 

let isObserverRoleCatState (roleId:int) (catId:int)  (stateId:int )  (ctx:DbContext) =
    let observer = ctx.Public.Observers |> Seq.tryFind (fun (x:Observer) -> (x.Roleid = roleId && x.Stateid = stateId && x.Categoryid = catId))
    match observer with
    | Some _ -> true
    | _ -> false

let isEnablerRoleCatState (roleId:int) (catId:int) (stateId:int )  (ctx:DbContext) =
    let enabler = ctx.Public.Enablers |> Seq.tryFind (fun (x:Enabler) -> (x.Roleid = roleId && x.Stateid = stateId && x.Categoryid = catId))
    match enabler with
    | Some _ -> true
    | _ -> false

let isObserverForRoleState (roleId:int) (stateId: int) (ctx:DbContext) =
    let observer = ctx.Public.Observers |> Seq.tryFind (fun (x:Observer) -> (x.Roleid = roleId && x.Stateid = stateId))
    match observer with
    | Some _ -> true
    | _ -> false

let isEnablerForRoleState (roleId:int) (stateId: int) (ctx:DbContext) =
    let enabler = ctx.Public.Enablers |> Seq.tryFind (fun (x:Enabler) -> (x.Roleid = roleId && x.Stateid = stateId))
    match enabler with
    | Some _ -> true
    | _ -> false


let safeRemoveOrderItem orderItemId (ctx:DbContext) =
    log.Debug (sprintf "%s %d" "safeRemoveOrderItem" orderItemId )
    let orderItem = ctx.Public.Orderitems |> Seq.find (fun (x:OrderItem) -> x.Orderitemid = orderItemId )

    // let states = orderItem.``public.orderitemstates by orderitemid``
    // states |> Seq.iter (fun (x:OrderItemState) -> x.Delete())
    // ctx.SubmitUpdates()

    // let variations = orderItem.``public.variations by orderitemid``
    // variations |> Seq.iter (fun (x:Variation) -> x.Delete())
    // ctx.SubmitUpdates()

    // let ingredientDecrements = orderItem.``public.ingredientdecrement by orderitemid``
    // ingredientDecrements |> Seq.iter (fun (x:IngredientDecrement) -> x.Delete())
    // ctx.SubmitUpdates()

    // let rejectedOrderItems = orderItem.``public.rejectedorderitems by orderitemid``
    // rejectedOrderItems |> Seq.iter (fun (x:RejectedOrderItems) -> x.Delete())
    // ctx.SubmitUpdates()

    orderItem.Delete()
    ctx.SubmitUpdates()

let getOrderIdOfSubOrder subOrderId (ctx:DbContext) =
    let subOrder = Orders.getSubOrder  subOrderId ctx
    subOrder.Orderid

let setSubOrderAsPayed subOrderId (ctx:DbContext) =
    let subOrder = Orders.getSubOrder  subOrderId ctx
    let _ = subOrder.Payed <- true
    
    ctx.SubmitUpdates()

let setOrderAsPayed orderId (ctx:DbContext) =
    let order = Orders.getOrder  orderId ctx
    let _ = order.Archived <- true
    ctx.SubmitUpdates()
let splitOrderItemInToUnitaryOrderItems id  (ctx:DbContext) =
    log.Debug(sprintf "%s %d " "splitOrderItemInToUnitaryOrderItems" id)
    // let orderItem = Orders.getOrderItemById id ctx
    let theOrderItem = getTheOrderItemById id ctx
    let connectedOrderItemStates = theOrderItem.``public.orderitemstates by orderitemid`` |> Seq.toList
    let ingredientdecrements = theOrderItem.``public.ingredientdecrement by orderitemid`` |> Seq.toList
    let rejectOrderItems = theOrderItem.``public.rejectedorderitems by orderitemid`` |> Seq.toList
    let variations = theOrderItem.``public.variations by orderitemid`` |> Seq.toList
    let courseId = theOrderItem.Courseid
    let orderId = theOrderItem.Orderid
    let comment = theOrderItem.Comment

    let outGroupId = theOrderItem.Ordergroupid

    let clonedOrderItems = [1 .. theOrderItem.Quantity] |> List.map (fun _ -> createUnitaryNakedOrderItemByOrderId courseId orderId comment theOrderItem.Price outGroupId ctx) // |> List.fold (@) []
    
    let do_cloneIngredientDecrements = clonedOrderItems  |> List.map (fun (x:OrderItem) -> ingredientdecrements |> List.map (fun (y:IngredientDecrement) -> createClonedIngredientDecrement x.Orderitemid ((decimal) theOrderItem.Quantity)  y ctx)) |> List.fold (@) []
    let do_cloneVariations = clonedOrderItems |> List.map (fun (x:OrderItem) -> variations |> List.map  (fun (y:Variation) -> createClonedVariationOfOrderItem x.Orderitemid y.Ingredientid y.Tipovariazione ctx)) |> List.fold (@) []
    let do_cloneRejectedOrderItems = clonedOrderItems |> List.map (fun (x:OrderItem) -> rejectOrderItems |> List.map (fun (y:RejectedOrderItems) -> createClonedRejectedOrderItem x.Orderitemid y ctx )) |> List.fold (@) []
    let do_cloneOrderItemStates = clonedOrderItems |> List.map (fun (x:OrderItem) -> connectedOrderItemStates |> List.map (fun (y:OrderItemState) -> createClonedOrderItemState x.Orderitemid y ctx)) |> List.fold (@) []
    
    safeRemoveOrderItem theOrderItem.Orderitemid ctx


let getUserViewById userId (ctx:DbContext):UsersView =
    log.Debug("getUserViewById")
    ctx.Public.Usersview |> Seq.find (fun (x:UsersView) -> x.Userid = userId)

let getAllOrdersOfUser userId (ctx:DbContext) =
    ctx.Public.Orders |> Seq.filter (fun (x:Order) -> x.Userid = userId)

let getOrderItemOfGroup groupId (ctx:DbContext) =
    query {
        for item in ctx.Public.Orderitems do
            where (item.Ordergroupid = groupId)
            select item
    } |> Seq.toList



let safeRemoveOrder orderId (ctx:DbContext) =
    log.Debug(sprintf "%s %d" "safeRemoveOrder" orderId)
    let order = ctx.Public.Orders |> Seq.find (fun (x:Order) -> x.Orderid = orderId)

    // let orderItems = order.``public.orderitems by orderid`` 
    
    // let voidedOrdersList = order.``public.voidedorderslogbuffer by orderid``

    // let archivedOrdersList = order.``public.archivedorderslogbuffer by orderid``
    // let connectedInvoices  = order.``public.invoices by orderid``
    // let connectedSuborders = order.``public.suborder by orderid``

    // let connectedPaymentItems = order.``public.paymentitem by orderid``

    // let outGroupOrders = order.``public.orderoutgroup by orderid``
    
    // Seq.iter (fun (x:OrderItem) -> safeRemoveOrderItem x.Orderitemid ctx) orderItems

    // Seq.iter (fun (x:PaymentItem) -> x.Delete()) connectedPaymentItems
    // ctx.SubmitUpdates()

    // Seq.iter (fun (x:VoidedOrdersLogBuffer) -> x.Delete()) voidedOrdersList
    // ctx.SubmitUpdates()

    // Seq.iter (fun (x:ArchivedorderLogBuffer) -> x.Delete()) archivedOrdersList
    // ctx.SubmitUpdates()


    // Seq.iter (fun (x:SubOrder) ->  Orders.forceDeleteSubOrder x.Suborderid ctx) connectedSuborders
    // ctx.SubmitUpdates()

    // Seq.iter (fun (x:Invoice) -> x.Delete()) connectedInvoices
    // ctx.SubmitUpdates()

    // Seq.iter (fun (x:OrderOutGroup) -> x.Delete()) outGroupOrders
    // ctx.SubmitUpdates()

    order.Delete()
    ctx.SubmitUpdates()

let getAllActionableStateOfUser userId (ctx:DbContext) =
    ctx.Public.Waiteractionablestates |> Seq.filter (fun (x:WaiterActionableState) -> x.Userid = userId)

let getAllCoursesByCourseCategory id (ctx: DbContext) =
    ctx.Public.Courses |> Seq.filter (fun (x:Course) -> x.Categoryid = id) |> Seq.toList

let safeDeleteUser userId (ctx:DbContext) =
    log.Debug("safeDeleteUser")
    let user = getUserById userId ctx
    let connectedOrders = getAllOrdersOfUser userId ctx

    // let connectedIngredientDecrements = user.``public.ingredientdecrement by userid``

    let connectedIngredientIncrements = user.``public.ingredientincrement by userid``
    Seq.iter (fun (x:Order) -> safeRemoveOrder x.Orderid ctx) connectedOrders
    let connecteActionableStates = getAllActionableStateOfUser userId ctx
    Seq.iter (fun (x:WaiterActionableState) -> x.Delete()) connecteActionableStates
    // Seq.iter (fun (x:IngredientDecrement) ->  x.Delete()) connectedIngredientDecrements
    Seq.iter (fun (x:IngredientIncrement) ->  x.Delete()) connectedIngredientIncrements
    ctx.SubmitUpdates()
    user.Delete()
    ctx.SubmitUpdates()

let getAllUsersByRole roleId (ctx:DbContext) =
    log.Debug("getAllUsersByRole")
    ctx.Public.Users |> Seq.filter (fun (x:User) -> x.Role  = roleId)

let getAllEnablersByRole roleId (ctx:DbContext) =
    log.Debug("getAllEnablersByRole")
    ctx.Public.Enablers |> Seq.filter (fun (x:Enabler) -> x.Roleid = roleId)

let getAllObserversByRole roleId (ctx:DbContext) =
    log.Debug("getAllObserversByRole")
    ctx.Public.Observers |> Seq.filter (fun (x:Observer) -> x.Roleid = roleId)

let getAllEnablersNamesByRole roleId (ctx:DbContext) =
    log.Debug("getAllEnablersNamesByRole")
    ctx.Public.Enablers |> Seq.filter (fun (x:Enabler) -> x.Roleid = roleId)

let safeDeleteRole roleId (ctx:DbContext) =
    log.Debug("safeDeleteRole")
    let role = getRole roleId ctx
    if (role.Rolename <> "admin" && role.Rolename <> "powerUser") then
        let connectedUsers = getAllUsersByRole roleId ctx
        Seq.iter (fun (x:User) -> (safeDeleteUser x.Userid ctx)) connectedUsers
        let connectedEnablers = getAllEnablersByRole roleId ctx
        Seq.iter (fun (x:Enabler) -> x.Delete()) connectedEnablers
        ctx.SubmitUpdates()
        let connectedObservers = getAllObserversByRole roleId ctx
        Seq.iter (fun (x:Observer) -> x.Delete()) connectedObservers
        ctx.SubmitUpdates()
        role.Delete()
        ctx.SubmitUpdates()
    else ()

let getAllOrderItemsOfCourse courseId (ctx:DbContext) =
    ctx.Public.Orderitems |> Seq.filter (fun (x:OrderItem) -> x.Courseid = courseId )

let getAllIngredientCourse courseId (ctx:DbContext) =
    ctx.Public.Ingredientcourse |> Seq.filter  (fun (x:IngredientCourse) -> x.Courseid = courseId )
let getAllRejectedOrderItemOfACourse courseId (ctx:DbContext) =
    ctx.Public.Rejectedorderitems |> Seq.filter (fun (x:RejectedOrderItems) -> x.Courseid = courseId)

let safeDeleteCourse (course:Course) (ctx:DbContext) =
    log.Debug("safeDeleteCourse")
    // let connectedOrderItems = getAllOrderItemsOfCourse course.Courseid ctx
    // let rejectedOrderItemlist = getAllRejectedOrderItemOfACourse course.Courseid ctx

    // Seq.iter (fun (x:RejectedOrderItems) -> x.Delete()) rejectedOrderItemlist
    // ctx.SubmitUpdates()

    // Seq.iter (fun (x:OrderItem) -> safeRemoveOrderItem x.Orderitemid ctx ) connectedOrderItems
    // ctx.SubmitUpdates()

    // let connectedIngredientCourses = getAllIngredientCourse course.Courseid ctx
    // Seq.iter (fun (x:IngredientCourse) -> x.Delete()) connectedIngredientCourses
    // ctx.SubmitUpdates()

    course.Delete()
    ctx.SubmitUpdates()

let getAllEnablersByCourseCategory id (ctx: DbContext) =
    ctx.Public.Enablers |> Seq.filter (fun (x:Enabler) -> x.Categoryid = id) |> Seq.toList
let getAllObserversByCourseCategory  id (ctx: DbContext) =
    ctx.Public.Observers |> Seq.filter (fun (x:Observer) -> x.Categoryid = id) |> Seq.toList

let getPrinterForCatgoryMappings categoryId (ctx: DbContext) =
    log.Debug("getPrinterForCatgoryMappings")
    query {
        for printerForCategory in ctx.Public.Printerforcategory do
        where (printerForCategory.Categoryid = categoryId)
        select  printerForCategory
    } |> Seq.toList

let safeDeleteCourseCategory id (ctx: DbContext) =
    log.Debug("safeDeleteCourseCategory")
    let courseCategory = Courses.tryGetCourseCategory id ctx
    match courseCategory with
    | Some theCourseCategory ->
        let connectedCourses = getAllCoursesByCourseCategory id ctx
        connectedCourses |> List.iter (fun (x:Course) -> safeDeleteCourse x ctx)
        ctx.SubmitUpdates()
        let connectedEnablers = getAllEnablersByCourseCategory id ctx
        connectedEnablers |> List.iter (fun (x:Enabler) -> x.Delete())
        ctx.SubmitUpdates()
        let connectedObservers = getAllObserversByCourseCategory id ctx
        connectedObservers |> List.iter (fun (x:Observer) -> x.Delete())
        ctx.SubmitUpdates()
        let connectedPrinterForCategory = getPrinterForCatgoryMappings id ctx
        connectedPrinterForCategory |> List.iter (fun (x:PrinterForCourseCategory) -> x.Delete())
        ctx.SubmitUpdates()

        theCourseCategory.Delete()
        ctx.SubmitUpdates()
    | None -> () 

let getAllTenderCodes (ctx:DbContext) =
    ctx.Public.Tendercodes |> Seq.toList

let addPaymentItemToSubOrder subOrderId tenderId amount (ctx:DbContext) =
    log.Debug("addPaymentItemToSubOrder")
    let newPaymentItem = ctx.Public.Paymentitem.``Create(amount, tendercodesid)``(amount,tenderId)
    let _ = newPaymentItem.Suborderid <- subOrderId
    ctx.SubmitUpdates()

let addPaymentItemToOrder orderId tenderId amount (ctx:DbContext) =
    log.Debug("addPaymentItemToOrder")
    let newPaymentItem = ctx.Public.Paymentitem.``Create(amount, tendercodesid)``(amount,tenderId)
    let _ = newPaymentItem.Orderid <- orderId
    ctx.SubmitUpdates()

let getIngredientsByCategory categoryId (ctx:DbContext) =
    ctx.Public.Ingredient |> Seq.filter (fun (x:Ingredient) -> x.Ingredientcategoryid = categoryId)
let getVariationByIngredient ingredientId (ctx: DbContext) =
    ctx.Public.Variations |> Seq.filter (fun (x:Variation) -> x.Ingredientid = ingredientId)
let getIngredientCourseByIngredient ingredientId (ctx: DbContext) =
    ctx.Public.Ingredientcourse |> Seq.filter (fun (x:IngredientCourse) -> x.Ingredientid = ingredientId)

let getIngredientCourseByCourseIdAndIngredientId courseId ingredientId (ctx:DbContext) =
    log.Debug("getIngredientCourseByCourseIdAndIngredientId\n")
    query {
        for ingredientCourse in ctx.Public.Ingredientcourse do
        where (ingredientCourse.Courseid = courseId && ingredientCourse.Ingredientid = ingredientId)
    } |> Seq.head

let getVariation variationId (ctx:DbContext) =
    log.Debug("getVariation")
    ctx.Public.Variations |> Seq.find (fun (x:Variation) -> x.Variationsid = variationId)
let getVariationDetail variationId (ctx:DbContext) =
    log.Debug("getVariationDetail")
    ctx.Public.Variationdetails |> Seq.find (fun (x:VariationDetail) -> x.Variationsid = variationId)

let tryGetIngredientCourseByCourseIdAndIngredientId courseId ingredientId (ctx:DbContext) =
    query {
        for ingredientCourse in ctx.Public.Ingredientcourse do
        where (ingredientCourse.Courseid = courseId && ingredientCourse.Ingredientid = ingredientId)
    } |> Seq.tryHead

let safeDeleteIngredient ingredientId (ctx: DbContext) =
    log.Debug("safeDeleteIngredient")
    let ingredient = getIngredientById ingredientId ctx
    let connectedVariations = getVariationByIngredient ingredientId ctx
    let connectedDecrements = ingredient.``public.ingredientdecrement by ingredientid``
    let connectedIngredientPrices = ingredient.``public.ingredientprice by ingredientid``
    connectedIngredientPrices |> Seq.iter (fun (x:IngredientPrice) -> x.Delete())
    connectedVariations |> Seq.iter (fun (x:Variation) -> x.Delete())
    connectedDecrements |> Seq.iter (fun (x:IngredientDecrement) -> x.Delete())
    ctx.SubmitUpdates()

    let connectedIncrements = ingredient.``public.ingredientincrement by ingredientid``
    connectedIncrements |> Seq.iter (fun (x:IngredientIncrement) -> x.Delete())
    ctx.SubmitUpdates()

    let connectedIngredientCourse = getIngredientCourseByIngredient ingredientId ctx
    connectedIngredientCourse |> Seq.iter (fun (x:IngredientCourse) -> x.Delete())
    ctx.SubmitUpdates()
    ingredient.Delete()
    ctx.SubmitUpdates()

let safeDeleteIngredientCategory id (ctx: DbContext) =
    let ingredientCategory = getIngredientCatgory id ctx
    let connectedIngredients = getIngredientsByCategory id ctx
    connectedIngredients |> Seq.iter (fun (x:Ingredient) -> safeDeleteIngredient x.Ingredientid ctx)
    ingredientCategory.Delete()
    ctx.SubmitUpdates()

let pushArchivedOrdersInLog orderId (ctx:DbContext) =
    let archivedEntry = ctx.Public.Archivedorderslogbuffer.Create(System.DateTime.Now,orderId)
    ctx.SubmitUpdates()

let addVoidedOrderToLog orderId userId (ctx:DbContext) =
    log.Debug("addVoidedOrderToLog")
    let newVoidLogEntry = ctx.Public.Voidedorderslogbuffer.Create(orderId, userId, System.DateTime.Now)
    ctx.SubmitUpdates()

let getTemporaryUserViewByName name (ctx:DbContext) =
    log.Debug("getTemporaryUserViewByName")
    let user = ctx.Public.Usersview |> Seq.tryFind (fun (x:UsersView)-> x.Istemporary && x.Username=name)
    match user with
    | Some theUser when (((theUser.Creationtime.AddMinutes(Globals.EXPIRATION_TIME_TEMPORARY_USERS)).CompareTo(System.DateTime.Now))>0) -> user
    | _ -> None

let makeUserConsumed userId (ctx:DbContext) =
    log.Debug("makeUserConsumed")
    let user = Users.getUser userId ctx
    user.Consumed <- true
    ctx.SubmitUpdates()

let getTemporaryUserByName name (ctx:DbContext) =
    log.Debug("getTemporaryUserByName")
    let user = ctx.Public.Users |> Seq.tryFind (fun (x:User)-> x.Istemporary && x.Username=name)
    match user with
    | Some theUser when (((theUser.Creationtime.AddMinutes(Globals.EXPIRATION_TIME_TEMPORARY_USERS)).CompareTo(System.DateTime.Now))>0) -> user
    | _ -> None

let getRoleByName name (ctx:DbContext) =
    log.Debug("getRoleByName")
    ctx.Public.Roles |> Seq.find (fun (x:Role) -> x.Rolename = name)  

let getCourseDetail courseId (ctx:DbContext) =
    log.Debug("getCourseDetail")
    ctx.Public.Coursedetails2 |> Seq.find (fun (x:Coursedetails) -> courseId = x.Courseid) 

let getOrderDetail id (ctx:DbContext) =
    log.Debug("getOrderDetail")
    ctx.Public.Orderdetails |> Seq.find (fun (x:Orderdetail) -> x.Orderid = id)

let setOrderItemAsRejected orderItemId (ctx: DbContext) =
    log.Debug("setOrderItemAsRejected")
    let orderItem = Orders.getOrderItemById orderItemId ctx
    match orderItem with
    | Some theOrderItem ->
        theOrderItem.Hasbeenrejected <- true
        ctx.SubmitUpdates()
    | _ -> ()

let reinitializeOrderItemState (orderItemId:int) (ctx: DbContext) =
    log.Debug("reinitializeOrderItemState")
    let orderItem = Orders.getOrderItemById orderItemId ctx
    match orderItem with
    | Some theOrderItem ->
        theOrderItem.Stateid <- (States.getInitState ctx).Stateid
        ctx.SubmitUpdates()
        ()
    | None -> ()

let getLatestRejectionOfOrderItem (orderItemId: int) (ctx: DbContext) =
     log.Debug("getLatestRejectionOfOrderItem")
     query {
         for item in ctx.Public.Rejectedorderitems do
         where (item.Orderitemid = orderItemId)
         sortByDescending item.Timeofrejection
         select item 
     } |> Seq.tryHead

let createIngredientIncrement (ingredient:Ingredient) (quantity: decimal) (unitOfMeasure: string) comment userId (ctx: DbContext) =
    log.Debug("createIngredientIncrement")
    let now = System.DateTime.Now
    let increment = ctx.Public.Ingredientincrement.``Create(comment, ingredientid, quantity, registrationtime, unitofmeasure, userid)``(comment,ingredient.Ingredientid,quantity,now,unitOfMeasure,userId)
    ctx.SubmitUpdates()


let getPrintersForCategories printerId (ctx:DbContext) =
    log.Debug("getPrintersForCategories")
    query {
        for printerForCategory in ctx.Public.Printerforcategory do
        where (printerForCategory.Printerid = printerId)
        select printerForCategory
    } |> Seq.toList

let getPrinterForCategoriesDetails printerId (ctx:DbContext) =
    log.Debug("getPrinterForCategoriesDetails")
    query {
        for printerForCategory in ctx.Public.Printerforcategorydetail do
        where (printerForCategory.Printerid = printerId)
        select printerForCategory
    } |> Seq.toList

let getPrinterForCategory printerId categoryId stateId (ctx:DbContext) =
    log.Debug("getPrinterForCategory")
    query {
        for printerForCategory in ctx.Public.Printerforcategory do
        where (printerForCategory.Printerid = printerId && 
            printerForCategory.Categoryid = categoryId && printerForCategory.Stateid = stateId)
        select printerForCategory
    } |> Seq.tryHead

let createOrUpdatePrinterReceiptAssociation printerId (ctx:DbContext) =
    log.Debug("createOrUpdatePrinterReceiptAssociation")
    let existingPrinterReceiptAssociation = tryGetPrinterForReceiptAndInvoice printerId ctx
    match existingPrinterReceiptAssociation with
    | Some X -> X.Printreceipt <- true
    | None -> ctx.Public.Printerforreceiptandinvoice.Create(printerId,false,true) |> ignore
    ctx.SubmitUpdates()

let removePrinterReceiptAssociationIfExists printerId (ctx:DbContext) =
    log.Debug("removePrinterReceiptAssociationIfExists")
    let existingPrinterReceiptAssociation = tryGetPrinterForReceiptAndInvoice printerId ctx
    match existingPrinterReceiptAssociation with
    | Some X -> X.Printreceipt <- false; ctx.SubmitUpdates()
    | _ -> ()

let createOrUpdatePrinterInvoiceAssociation printerId (ctx:DbContext) =
    log.Debug("createOrUpdatePrinterInvoiceAssociation")
    let existingPrinterInvoiceAssociation = tryGetPrinterForReceiptAndInvoice printerId ctx
    match existingPrinterInvoiceAssociation with
    | Some X -> X.Printinvoice <- true
    | None -> ctx.Public.Printerforreceiptandinvoice.Create(printerId,true,false) |> ignore
    ctx.SubmitUpdates()

let removePrinterInvoiceAssociationIfExists printerId (ctx:DbContext) =
    log.Debug("removePrinterInvoiceAssociationIfExists")
    let existingPrinterInvoiceAssociation = tryGetPrinterForReceiptAndInvoice printerId ctx
    match existingPrinterInvoiceAssociation with
    | Some X -> X.Printinvoice <- false; ctx.SubmitUpdates()
    | _ -> ()


let createPrinterCategoryAssociation printerId categoryId (stateName:string) (ctx:DbContext) =
    log.Debug("createPrinterCategoryAssociation")
    let state = States.tryGetStateByName stateName ctx
    match state with
    | Some theState ->

        let existingprinterForCategory = getPrinterForCategory printerId categoryId theState.Stateid ctx
        match existingprinterForCategory with
        | None -> ctx.Public.Printerforcategory.``Create(categoryid, printerid, stateid)``(categoryId, printerId,theState.Stateid) |> ignore; ctx.SubmitUpdates() 
        | _ -> ()
    | _ -> ()

let removePrinterCategoryAssociation printerId categoryId (stateName:string) (ctx:DbContext) =
    log.Debug("removePrinterCategoryAssociation")
    let state = States.tryGetStateByName stateName ctx
    match state with
    | Some theState ->
        let existingprinterForCategory = getPrinterForCategory printerId categoryId theState.Stateid ctx
        match existingprinterForCategory with
        | Some X -> X.Delete()
                    ctx.SubmitUpdates()
        | None -> ()
    | _ -> ()

let getStatesPrinterMapping printerId (ctx:DbContext) =
    log.Debug("getStatesPrinterMapping")
    query  {
        for printerStateMapping in ctx.Public.Printerforcategorydetail do
            where (printerStateMapping.Printerid = printerId)
            select printerStateMapping
    } |> Seq.toList

let getIngredientDecrementsStartingFromDate ingredientId date (ctx: DbContext): IngredientDecrementView list =
    log.Debug("getIngredientDecrementsStartingFromDate")
    let myDate = System.DateTime.Parse(date,CultureInfo.CreateSpecificCulture("en-US"))
    ctx.Public.Ingredientdecrementview |> Seq.filter (fun (x:IngredientDecrementView) -> (x.Ingredientid = ingredientId && System.DateTime.Compare(x.Closingtime,myDate)>0)) |> Seq.toList

let addAmountOfingredientAvailability (ingredient:Ingredient) (amount:decimal) (ctx:DbContext) =
    log.Debug(sprintf "%s ingredientid: %d, quantity: %d " "addAmountOfingredientAvailability" ingredient.Ingredientid ((int)amount))
    ingredient.Availablequantity<- (ingredient.Availablequantity + amount)
    log.Debug("updated quantity")
    ctx.SubmitUpdates()



