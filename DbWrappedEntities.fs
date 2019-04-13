module OrdersSystem.DbWrappedEntities

open FSharp.Data.Sql
open OrdersSystem
open System
open FSharp.Configuration

open System.Globalization
open ExpressionOptimizer

type CourseWrapped = {
   CourseName: string;
   CourseId: int;
}

type PaymentItemWrapped = {
    Amount: decimal;
    TenderName: string;
    TenderCodeId: int;
    PaymentItemId: int;
    SubOrderId: int;
}

type TenderCodeWrapped = {
    TenderCodeName: string;
    TenderCodeId: int;
    TenderCodeIdentifier: int;
}

type OrderItemDetailsWrapped = {
    Quantity: int;
    Categoryid: int;
    Closingtime: DateTime;
    Comment: string;
    Courseid: int;
    Hasbeenrejected: bool;
    Name: string;
    Orderid: int;
    Orderitemid: int;
    Orderout: int;
    Originalprice: decimal;
    Price: decimal;
    Person: string;
    Suborderid: int;
    Isinasuborder: bool;
    Paid: bool;
    Csscolor: string;
    Totalprice: decimal;
    Stateid: int
}

type SubOrderWrapped = {
    Suborderid: int;
    Orderid: int;
    Comment: string; 
    Subtotal: decimal;
    Subtotaladjustment: decimal;
    SubtotalPercentAdjustment: decimal;
    SubtotalAdjustmentFromPercentage: decimal;
    Csscolor: string;
    Paid: bool
}

type OrderWrapped = {
    OrderId: int;
    Tavolo: string;
    Totale: decimal;
    Sconto: decimal;
    TotaleScontato: decimal
}


///
/// experimental feature
let wrapMyObject (object: Common.SqlEntity) =
    let table = object.Table
    match table.Name with
    | "orderitemdetails" ->  printf "ok"
    | _ -> printf "not ok"






type DbObjectWrapper =
    static member WrapOrderItemDetails(orderItemDetail: Db.OrderItemDetails) cssColor =
    {
        Quantity = orderItemDetail.Quantity;
        Categoryid = orderItemDetail.Categoryid;
        Closingtime = orderItemDetail.Closingtime;
        Comment  = orderItemDetail.Comment;
        Courseid = orderItemDetail.Courseid;
        Hasbeenrejected = orderItemDetail.Hasbeenrejected;
        Name = orderItemDetail.Name;
        Orderid = orderItemDetail.Orderid;
        Orderitemid = orderItemDetail.Orderitemid;
        Orderout= orderItemDetail.Groupidentifier;
        Originalprice=orderItemDetail.Originalprice;
        Price = orderItemDetail.Price;
        Person= orderItemDetail.Person;
        Suborderid = orderItemDetail.Suborderid;
        Isinasuborder = orderItemDetail.Isinsasuborder;
        Paid = orderItemDetail.Payed;
        Csscolor = cssColor;
        Totalprice = (decimal)orderItemDetail.Quantity * orderItemDetail.Price;
        Stateid = orderItemDetail.Stateid;

    }

    static member WrapCourse (course: Db.Course) =
    {
        CourseId = course.Courseid
        CourseName  = course.Name
    }

    static member WrapSubOrder (subOrder: Db.SubOrder) cssColor =
    {
        Suborderid = subOrder.Suborderid;
        Orderid = subOrder.Orderid;
        Comment = subOrder.Comment;
        Subtotal = subOrder.Subtotal;
        Subtotaladjustment = subOrder.Subtotaladjustment;
        SubtotalPercentAdjustment = subOrder.Subtotalpercentadjustment;
        SubtotalAdjustmentFromPercentage = Math.Round(subOrder.Subtotal * (subOrder.Subtotalpercentadjustment/100M),2,MidpointRounding.ToEven);
        Csscolor = cssColor;
        Paid = subOrder.Payed
    }
    
    static member WrapOrder (order: Db.Order) =
    {
        OrderId=order.Orderid;
        Tavolo=order.Table;
        Totale= order.Total;
        Sconto = order.Total - order.Adjustedtotal;
        TotaleScontato = order.Adjustedtotal;
    }

    static member WrapTenderCode (tenderCode:Db.TenderCode) =
    {
        TenderCodeName= tenderCode.Tendername;
        TenderCodeId=tenderCode.Tendercodesid;
        TenderCodeIdentifier=tenderCode.Tendercode
    }

    static member WrapPaymentItem (paymentItem:Db.PaymentItemDetail) =
    {
        SubOrderId = paymentItem.Suborderid;
        Amount = paymentItem.Amount;
        TenderCodeId= paymentItem.Tendercodesid;
        TenderName = paymentItem.Tendername;
        PaymentItemId = paymentItem.Paymentid;
    }










