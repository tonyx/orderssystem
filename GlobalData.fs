module OrdersSystem.Globals
// open Db
open FSharp.Collections
open Microsoft.FSharp.Data.UnitSystems.SI

type DecimalIntStringPrintFormat = PrintfFormat<(decimal->int->string->string),unit,string,string,(decimal*int*string)>
type DecimalStringPrintFormat = PrintfFormat<(decimal->string->string),unit,string,string,(decimal*string)>
type IntDecimalPrintFormat = PrintfFormat<(int->decimal->string),unit,string,string,(int*decimal)>

let ITALIAN_TEMPLATE_ROW_FOR_UNITARY_PAYMENT_ITEM:DecimalStringPrintFormat = "vend rep=1,pre=%.2f, des='%s'"
let ITALIAN_TEMPLATE_ROW_FOR_PAYMENT_ITEM:DecimalIntStringPrintFormat = "vend rep=1, pre=%.2f, qty=%d, des='%s'"
let ITALIAN_TEMPLATE_ROW_FOR_PAYMENT_CLOSURE_ITEM:IntDecimalPrintFormat = "chius t=%d, imp=%.2f";
let ITALIAN_TEMPLATE_ROW_FOR_PAYMENT_CLOSURE_WITH_CREDIT = "chius t =2"

// let me = 1.0 [kg]
let ALIQUOTA_IVA_UNICA = 10.0M


let NORMAL = "NORMAL"

[<Literal>]
let PER_PREZZO_INGREDIENTE = "PER_PREZZO_INGREDIENTE"

let SENZA =   "🚫" // \xF0\x9F\x98\x81" // U+1F601 //  "\xF0\x9F\x91\x87"   // "SENZA"
let ALT_SENZA = "SENZA"
let POCO = "✋" // "POCO"
let ALT_POCO = "POCO"
let MOLTO =  "😍" //  "MOLTO"

let ALT_MOLTO = "MOLTO"

let AGGIUNGIPOCO =  "✋" // "AGGIUNGI POCO" 
let UNITARIO = "unitary"

let ALT_AGGIUNGIPOCO = "AGGIUNGI POCO"
let AGGIUNGINORMALE = "👍" // "AGGIUNGI NORMALE"
let ALT_AGGIUNGINORMALE = "AGGIUNGI NORMALE"
let AGGIUNGIMOLTO = "😍" // "AGGIUNGI MOLTO"
let ALT_AGGIUNGIMOLTO = "AGGIUNGI MOLTO"
let ALERGEN_MARK = "(😤!)"

let ARCHIVED_ORDERS_BUFFER_SIZE = 10

let EXPIRATION_TIME_TEMPORARY_USERS = 8.0

let ELIMINA_ALLERGENI = " 🚫   😤 🤧"
let ALT_ELIMINA_ALLERGENI ="ELIMINA ALLERGENI"
let ELIMINA_INGREDIENTI_INVISIBILI = "elimina ingredienti indisponibili"
let NUM_DB_ITEMS_IN_A_PAGE = 10 

let EMOTICONS_SUBSTITUTIONS = [(SENZA,ALT_SENZA);(POCO,ALT_POCO);
  (MOLTO,ALT_MOLTO);(AGGIUNGIPOCO,ALT_AGGIUNGIPOCO);(AGGIUNGINORMALE,ALT_AGGIUNGINORMALE);(AGGIUNGIMOLTO,ALT_AGGIUNGIMOLTO) ] 

let rec substituteRec (strIn: string) (substitutions: (string*string) list) =

  match substitutions with
  |  (A,B)::T -> substituteRec (strIn.Replace(A,B)) T
  | [] -> strIn

let replaceEmojWithPlainText strIn = substituteRec strIn EMOTICONS_SUBSTITUTIONS

let stateDisplayNameToCodeName = Map.ofList [
    ("Collecting","COLLECTING");
    ("To be worked","TOBEWORKED");
    ("Started working","STARTEDWORKING");
    ("Ready for Delivery","READYFORDELIVERY");
    ("Delivered","DELIVERED");
    ("Enabler","ENABLER") ] 

let WEIGHT_UNIT_OF_MEASURES = ["gr";"dg";"hg";"dag"]
let LIQUID_UNIT_OF_MEASURES =  ["lt";"cl";"dl"]

[<Literal>]
let UNITARY_MEASUSERE = "unità"

let UNITARY_MEASURES = [UNITARY_MEASUSERE]


[<Measure>] 
 type kg =
  static member toGr (t: float<kg>) = 1000.0<gr>
  static member fromGr (t: float<gr>) = 0.001<kg>
  static member toHg (t: float<kg>) = 10.0<hg>
  static member fromHg (t: float<hg>) = 0.1<kg>
  static member toDag (t: float<kg>) = 0.01<dag>
  static member fromDag (t: float<dag>) = 10.0<dag>

 and
  [<Measure>] 
  gr =
    static member toKg = kg.fromGr
    static member fromKg = kg.toGr
    static member toHg (t: float<gr>) = 100.0<gr>
    static member fromHg (t: float<hg>) = 0.01<hg>
    static member toDag  (t: float<gr>) = 10.0<dag>
    static member fromDag (t: float<dag>) = 0.1<gr>

 and 
  [<Measure>]
  hg = 
    static member toKg = kg.fromHg
    static member fromKg = kg.toHg
    static member toGr = gr.fromHg
    static member fromGr = gr.toHg
    static member toDag  (t:float<hg>) = 10.0<dag>
    static member fromDag  (t:float<dag>) = 0.1<hg>

 and 
  [<Measure>]  
  dag =
    static member toKg = kg.fromDag     
    static member fromKg = kg.toDag 
    static member toGr = gr.fromDag     
    static member fromGr = gr.toDag 
    static member toHg = hg.fromDag     
    static member fromHg = hg.toDag 



[<Measure>]
  type lt =    
    static member toCl (t: float<lt>) = 100.0<cl>
    static member fromCl (t: float<cl>) = 0.01<lt>
    static member toDl (t:float<lt>) = 0.1<dl>
    static member fromDl (t:float<dl>) = 0.1<lt>
  and
   [<Measure>]
    cl =
        static member toLt = lt.fromCl
        static member fromLt = lt.toCl
        static member toDl (t: float<cl>) = 0.1<dl>
        static member fromDl (t: float<dl>) = 10.0<dl>
  and 

    [<Measure>]
     dl = 
         static member toLt = lt.fromDl
         static member fromLt = lt.toDl
         static member toCl = cl.fromDl
         static member fromCl = cl.toDl




let stylesForSubOrders = [
  "subordercolorA";
  "subordercolorB";
  "subordercolorC";
  "subordercolorD";
  "subordercolorE";
  "subordercolorF";
  "subordercolorG";
  "subordercolorH";
]



let colourValues = [
        "#00FF00"; "#0000FF"; "#FFFF00"; "#FF00FF"; "#00FFFF";  
        "#008000"; "#000080"; "#808000"; "#800080"; "#008080"; "#808080"; 
        "#C00000"; "#00C000"; "#0000C0"; "#C0C000"; "#C000C0"; "#00C0C0"; "#C0C0C0"; 
        "#004000"; "#000040"; "#404000"; "#400040"; "#004040"; "#404040"; 
        "#002000"; "#000020"; "#202000"; "#200020"; "#002020"; "#202020"; 
        "#600000"; "#006000"; "#000060"; "#606000"; "#600060"; "#006060"; "#606060"; 
        "#A00000"; "#00A000"; "#0000A0"; "#A0A000"; "#A000A0"; "#00A0A0"; "#A0A0A0"; 
        "#E00000"; "#00E000"; "#0000E0"; "#E0E000"; "#E000E0"; "#00E0E0"; "#E0E0E0";"#FF0000"; 
    ]

let getFirstNColorValues (n: int) =
    List.take n colourValues











