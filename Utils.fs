
module OrdersSystem.Utils

open OrdersSystem
open ExpressionOptimizer


let javascriptDecimalStringPairMapConverter (l:(decimal*string) list) =
    "new Map(["+ (l |> List.fold (fun acc (x,y) -> acc + "["+ "\""  + (string)  x+ "\"" +  ","+"\""+ y+ "\"" + "],") "")+"]);"


let intPairsMapToJavascriptString l =
    let javascriptListPairs l = l |> List.fold (fun acc (x,y)  -> acc + "["+(string)x+","+(string)y+"],") ""

    let rec javascriptIntTolistPairsMap l acc =
        match l with
        | (A : int, B : ((int*decimal) List)):: T -> javascriptIntTolistPairsMap T  acc+"[" + (string) A + ", " + "new Map(["+ (javascriptListPairs B ) +  "])],"
        | _ -> acc

    "new Map([" +  (javascriptIntTolistPairsMap l "") + "]);"

let unbundleVat price rate =
    let unbundlerMultiplier = 100M/(100M+rate)
    let unbundledPrice = price*unbundlerMultiplier
    unbundledPrice

let textForWholeOrderReceipt orderId (orderItemsDetails:Db.OrderItemDetails list)  (ctx:Db.DbContext) =
    let order = Db.Orders.getOrder orderId ctx
    let textAboutTotal =   
        match (order.Adjustispercentage,order.Adjustisplain) with
        | (true,false) -> sprintf "sconto percentuale: %.2f%%\n%s %.2f"  order.Percentagevariataion "Totale scontato" order.Adjustedtotal
        | (false,true) -> sprintf "sconto: %.2f\n%s %.2f"  order.Plaintotalvariation  "Totale scontato" order.Adjustedtotal
        | _ -> ""

    let now = System.DateTime.Now.ToLocalTime()

    let text = "riepilogativo non fiscale:\n\n"+ "data:"+now.ToString()+"\n\n"+
        (orderItemsDetails |> (List.fold (fun y (x:Db.OrderItemDetails) ->  y + 
        (sprintf "%d %-20s %-10.2f\n"   x.Quantity  x.Name  x.Price )) ""))  +  "\nTotale: "+(order.Total |> string)+ "\n"+textAboutTotal

    text




let textForSubOrderReceipt  (orderItemsDetails:Db.OrderItemDetails list)  (ctx:Db.DbContext) =

    // let order = Db.getOrder orderId ctx

    let total = orderItemsDetails |> List.map (fun (x:Db.OrderItemDetails) -> x.Price) |> List.fold (+) 0.0M

    // let textAboutTotal =   
    //     match (order.Adjustispercentage,order.Adjustisplain) with
    //     | (true,false) -> sprintf "sconto percentuale: %.2f%%\n%s %.2f"  order.Percentagevariataion "Totale scontato" order.Adjustedtotal
    //     | (false,true) -> sprintf "sconto: %.2f\n%s %.2f"  order.Plaintotalvariation  "Totale scontato" order.Adjustedtotal
    //     | _ -> ""

    let now = System.DateTime.Now.ToLocalTime()

    let text = "riepilogativo non fiscale:\n\n"+ "data:"+now.ToString()+"\n\n"+
        (orderItemsDetails |> (List.fold (fun y (x:Db.OrderItemDetails) ->  y + 
        (sprintf "%d %-20s %-10.2f\n"   x.Quantity  x.Name  x.Price )) ""))  +  "\nTotale: "+(sprintf "%.2f" total)

        //  (order.Total |> string)+ "\n"+textAboutTotal

    text




let variationsByStringDescription (listOfVariations:(int*Db.VariationDetail list) list) (ctx:Db.DbContext)= 
    listOfVariations |>
        List.map (fun (id, (variations:Db.VariationDetail list)) -> 
            (id,List.fold (fun y (v:Db.VariationDetail) ->  
            (if (v.Tipovariazione <> Globals.UNITARY_MEASUSERE && v.Tipovariazione <> Globals.PER_PREZZO_INGREDIENTE  ) then 
                v.Tipovariazione  // molto poco etc...
              else 
                ( if (v.Tipovariazione <> Globals.PER_PREZZO_INGREDIENTE)  then (
                    ((match (Db.tryGetIngredientCourseByCourseIdAndIngredientId v.Courseid v.Ingredientid ctx) with 
                    | Some X -> X.Quantity 
                    | None -> (decimal)0.0 ) 
                    + ((decimal)v.Plailnumvariation) |> int |> string) 
                    ) 
                    else 
                      let ingredientPrice = Db.getIngredientPrice v.Ingredientpriceid ctx
                      (string) (ingredientPrice.Quantity)
                    
                    //   "UNIMPLEMENTED"
                )
            ) 
                + " "+v.Ingredientname + ", " + y) "" variations)
            )
            |> Map.ofList
