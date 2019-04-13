
module OrdersSystem.Path

type IntPath = PrintfFormat<(int -> string),unit,string,string,int>
type IntPath2 = PrintfFormat<(int  -> int -> string),unit,string,string,(int*int)>
type IntPath3 = PrintfFormat< (int  -> int -> int ->  string),unit,string,string,(int*int*int)>
type IntPath4 = PrintfFormat< (int  -> int -> int -> int -> string),unit,string,string,(int*int*int*int)>

type StrPath = PrintfFormat<(string -> string),unit,string,string,string>
type IntStrPath = PrintfFormat<(int -> string -> string),unit,string,string,(int*string)>
type Int2StrPath = PrintfFormat<(int -> int -> string -> string),unit,string,string,(int*int*string)>
type Int3StrPath = PrintfFormat<(int -> int -> int -> string -> string),unit,string,string,(int*int*int*string)>

let withParam (key,value) path = sprintf "%s?%s=%s" path key value
let with2Params (key1,value1) (key2,value2) path = sprintf "%s?%s=%s&%s=%s" path key1 value1 key2 value2

let home = "/"

module Errors =
    let unableToCompleteOperation = "/errors/unabletoCompleteOperation"

module Orders =

    let removeAllDiscountOfSubOrder: IntPath = "/orders/removeAllDiscountsOfSubOrder/%d" 
    let removePaymentItemOfSubOrder: IntPath3 = "/orders/removePaymentItemOfSubOrder/%d/%d/%d"
    let removePaymentItemOfOrder: IntPath2 = "/orders/removePaymentItemOfOrder/%d/%d"
    let createEcrReceiptInstructionForSubOrder: IntPath2 = "/orders/createEcrReceiptInstructionForSubOrder/%d/%d"
    let createEcrReceiptInstructionForOrder: IntPath = "/orders/createEcrReceiptInstructionForOrder/%d"
    let wholeOrderPaymentItems : IntPath = "/orders/wholeOrderPaymentItems/%d"
    let subOrderPaymentItems: IntPath2 = "/orders/subOrderPaymentItems/%d/%d" 
    let printWholeOrderInvoice: IntPath = "/orders/printWholeOrderInvoice/%d"
    let printWholeOrderReceipt: IntPath = "/orders/printWholeOrderReceipt/%d"
    let rejectOrderItem: IntPath = "/orders/rejectOrderItem/%d"
    let unVoidLatestVoidedOrderOfUser: IntStrPath = "/orders/unVoidLatestVoidedOrderOfUser/%d/%s"
    let dearchiveLatestOrder = "/orders/deArchiveLatestOrder"
    let removeIngredientVariation: Int2StrPath = "/orders/removeIngredientVariation/%d/%d/%s"
    let increaseUnitaryIngredientVariation: IntStrPath = "/orders/increaseUnitaryIngredientVariation/%d/%s"
    let decreaseUnitaryIngredientVariation: IntStrPath = "/orders/decreaseUnitaryIngredientVariation/%d/%s"
    let addIncreaseIngredientVariation: Int2StrPath = "/orders/addMoreIngredientVariation/%d/%d/%s" 
    let addDiminuishIngredientVariation: Int2StrPath = "/orders/addDiminuishIngredientVariation/%d/%d/%s" 
    let addWithougIngredientVariation: Int3StrPath = "/orders/addWithoutIngredientVariation/%d/%d/%d/%s" 
    let addOrder = "/orders/addOrder"
    let addSingleOrder = "/orders/addSingleOrder"
    let myOrders  = "/orders/myOrders"
    let myOrdersSingles  = "/orders/myOrdersSingles"
    let moveAllInitialStateOrderItems: IntPath = "/orders/moveAllInitialtateOrderItems/%d"
    let moveInitialStateOrderItemsByOutGroup: Int2StrPath = "/orders/moveInitialStateOrderItemsByOutGroup/%d/%d/%s"
    let reprintOrderItemsGroup: Int2StrPath = "/orders/reprintOrderItemsGroup/%d/%d/%s"
    let addOrderItemByCategory: Int2StrPath  = "/orders/addOrderItemByCategory/%d/%d/%s" 
    let deleteOrderItem: IntStrPath = "/orders/deleteOrderItem/%d/%s"
    let removeAllAllergenicFromOrderItem: IntStrPath = "/order/removeAllergenicFromOrderItem/%d/%s"
    let removeAllUnavailableIngredientsFromOrderItem: IntStrPath = "/order/removeAllUnavailablesFromOrderItem/%d/%s"
    let editOrderItemVariation: IntStrPath = "/orders/editOrderItemVariation/%d/%s"
    let editOrderItemVariationByIngredientCategory: Int2StrPath = "/orders/editOrderItemVariationWithIngredentCategory/%d/%d/%s"
    let editOrderItemByCategory: Int2StrPath = "/editOrderItmeByCategory/%d/%d/%s"
    let resetVariationsAndEditOrderItemByCategory: Int2StrPath = "/resetVarAndeditOrderItmeByCategory/%d/%d/%s"
    let editDoneOrderitem: IntPath2 = "/editDoneOrderItem/%d/%d"
    let resetDiscount: IntPath = "/orders/resetDiscount/%d"

    let editDoneOrderItemFromSubSplitting:  IntPath3 = "/orders/editDoneOrderItemFromSubSplitting/%d/%d/%d"
    let moveOrderItemToTheNextStateAndGoMyOrders: IntStrPath = "/orders/moveOrderItemNextStepThenMyOrders/%d/%s"
    let removeOrderItemThenGoBackToUrl: IntStrPath = "/orders/removeOrderItemThenGoBackToUrl/%d/%s"
    let moveOrderItemToTheNextStateAndGoOrdersProgress: IntPath = "/orders/moveOrderItemNextStepThenOrderProgress/%d"
    let orderItemsProgress = "/orders/orderItemsProgress"
    let confirmVoidOrderFromMyOrders: IntStrPath = "/orders/me/confirmVoidOrder/%d/%s"
    let voidOrderFromMyOrders: IntStrPath = "/orders/me/voidOrder/%d/%s"
    let allOrders = "/orders/allOrders"
    let seeDoneOrders = "/orders/seeDoneOrders"
    let achiveOrder: IntPath = "/orders/archive/%d"
    let editDoneOrder: IntPath = "/orders/editDoneOrder/%d"
    let subdivideDoneOrder: IntPath = "/orders/subdivideDoneOrder/%d"
    let colapseDoneOrder: IntPath = "/orders/colapseDoneOrder/%d"
    let splitOrderItemInToUnitaryOrderItems: IntPath = "/orders/splitOrderItemInToUnitaryOrderItems/%d"
    let deleteSubOrder: IntPath2 = "/orders/deleteSubOrder/%d/%d"
    let printSubOrderInvoice: IntPath = "/orders/printSubOrderInvoice/%d"
    let printInvoice: IntPath2 = "/orders/printInvoice/%d/%d"
    let printReceipt: IntPath2 = "/orders/printReceipt/%d/%d"
    let setSubOrderAsPaid: IntPath2 = "/orders/setSubOrderAsPaid/%d/%d"
    let setSubOrderAsNotPaid: IntPath2 = "/orders/setSubOrderAsNotPaid/%d/%d"
    let viewOrder: IntPath = "/orders/viewOrder/%d"
    let selectOrderItemsToAddFromAnotherOrder: IntPath2 = "/orders/selectOrderItemsToAddFromAnotherOrder/%d/%d"
    let selectOrderFromWhichMoveOrderItems: IntPath = "/orders/selectOrderFromWhichMoveOrderItems/%d"
    
module Admin =
    let removePrinter: IntPath = "/admin/removePrinter/%d"
    let deleteIngredientPrice: IntPath = "/admin/deleteIngredientPrice/%d"
    let deleteIngredient: IntPath = "/admin/deleteIngredient/%d"
    let deleteIngredients = "/admin/deleteIngredients"
    let deleteRole: IntPath = "/admin/deleteUserRole/%d"
    let deleteUserRoles = "/admin/deleteUserRoles"
    let deleteCourseCategory: IntPath = "/admin/deleteCourseCategory/%d"
    let deleteCourses = "/admin/deleteCourses"
    let deleteUser : IntPath = "/admin/deleteuser/%d"
    let deleteIngredientCategory: IntPath = "/admin/deleteIngredientCategory/%d"
    let deleteUsers = "/admin/deleteUsers"
    let deleteTemporaryUsers = "/admin/deleteTemporaryUsers"
    let deleteIngredientCategories = "/admin/deleteIngreientCategories"
    let deleteCourseCategories = "/admin/deleteCourseCategories"
    let deleteObjects = "/admin/deleteObjects"
    let optimizeVoided = "/admin/optimizeVoided"
    let switchVisibilityOfIngredientCategory: IntStrPath = "/admin/switchIngredientCategoryVisibility/%d/%s"
    let editIngredient: IntPath2 = "/admin/editIngredient/%d/%d"
    let viewIngredientUsage: IntPath2 = "/admin/viewIngredientUsage/%d/%d"
    let fillIngredient: IntPath2 = "/admin/fillIngredient/%d/%d"
    let editIngredientPrices: IntPath = "/admin/editIngredientPrices/%d"
    let loadIngredient: IntPath2 = "/admin/loadIngredient/%d/%d"
    let switchVisibilityOfIngredient: IntPath2 = "/admin/switchIngVisibilityOfIngredient/%d/%d"
    let editIngredientCategory: IntPath = "/admin/ingredientCategory/%d"
    let editIngredientCategoryPaginated: IntPath2 = "/admin/ingredientCategoryPaginated/%d/%d"
    let resetPrinters = "/admin/resetPrinters"
    let recognizePrinters = "/admin/recognizePrinters"
    let managePrinter: IntPath2 = "/admin/managePrinter/%d/%d"
    let info = "/admin/info"
    let printers = "/admin/printers"
    let allIngredientCategories = "/admin/allIngredientCategories"
    let visibleIngredientCategories = "/admin/visibleIngredientCategories"
    let roles = "/admin/roles"
    let addRole = "/admin/addRole"
    let tempUserDefaultActionableStates = "/admin/tempUserDefaultActionableStates"
    let defaultActionableStatesForOrderOwner = "/admin/defaultActionableStatesForOrderOwner"
    let actionableStatesForSpecificOrderOwner: IntPath = "/admin/specificOwnerActionables/%d"
    let setRoleasObserverForCourseCategory = "/roles/setRoleAsObserverForCourseCatogory"
    let setStateAsActionableForWaiterByDefault: IntPath = "/roles/setDefaultStateStateForWaiter/%d"
    let unSetStateAsActionableForWaiterByDefault: IntPath = "/roles/unSetDefaultStateStateForWaiter/%d"
    let setStateAsActionableForTempUserByDefault: IntPath = "/roles/setStateAsActionableForTempUserByDefault/%d"
    let unSetStateAsActionableForTempUserByDefault: IntPath = "/roles/unSetStateAsActionableForTempUserByDefault/%d"
    let setStateAsActionableForSpecificWaiter: IntPath2 = "/roles/setActionStateStateForSpecificWaiter/%d/%d"
    let unSetStateAsActionableForSpecificWaiter: IntPath2 = "/roles/unSetActionStateForSpecificWaiter/%d/%d"
    let setRoleasEnablerForCourseCategory = "/roles/setRoleAsEnablerForCourseCatogory"
    let roleEnablerObserverCategoriesByCheckBoxes = "/roles/roleEnablerObserverCategoriesByCheckBoxes"
    let roleEnablerObserverCategoriesByCheckBoxesByRoleAndCat: IntPath2 = "/roles/roleEnablerObserverCategoriesByCheckBoxesRoleCat/%d/%d"
    let removeObserverMapping: IntPath = "/admin/roles/removeObserver/%d"
    let removeEnablerMapping: IntPath = "/admin/roles/removeEnabler/%d"
    let addUser = "/admin/users/addUser"
    let editUser: IntPath  = "/admin/editUser/%d"
    let editTemporaryUser: IntPath = "/admin/editTemporaryUser/%d"
    let allUsers = "/admin/users"
    let temporaryUsers = "/admin/temporaryUsers"


module Courses =
    let deleteIngredientToCourse: IntPath2 = "/courses/deleteingredientToCourse/%d/%d"
    let selectAllIngredientsForCourseEdit: IntPath = "/courses/selectAllIngredientsForCourseEdit/%d"
    let selectIngredientCategoryForCourseEdit: IntPath2 = "/course/selectIngredientCategoryForCourseEdit/%d/%d"
    let addCourseByCategory: IntPath = "/courses/addCourseByCategory/%d"
    let manageAllCoursesOfACategory: IntPath = "/courses/allCoursesByCategory/%d"
    let manageAllCoursesOfACategoryPaginated: IntPath2 = "/courses/allCoursesByCategoryPaginated/%d/%d"
    let switchCourseCategoryVisibility: IntPath = "/admin/courses/switchCourseCategoryVisibility/%d"
    let manageVisibleCoursesOfACategoryPaginated: IntPath2 = "/courses/visibleCoursesByCategoryPaginated/%d/%d"
    let manageAllCourses = "/courses/manageAllCourses"
    let adminCategories = "/courses/adminCategories"
    let editCategory: IntPath = "/courses/editCategory/%d"
    let editCourse: IntPath = "/courses/editCourse/%d"
    let manageVisibleCoursesOfACategory: IntPath = "/courses/visibleCoursesByCategory/%d"
    let addCategory = "/courses/addCategory"
    let manageAllCategories = "/courses/manageAllcategories"

module Account =
    let logon = "/account/logon"
    let logoff = "/account/logoff"
    let changePassword = "/account/changePassword"


module Extension =
    let qrCodeLogin = "/qrCodeLogin"
    let addQruser  = "/addQrUser"
    let qrUserOrder = "/qrUser/order"
    let qrUserImageGen = "/qrUser/qrUserImageGen"
    let regenTempUser: IntPath = "/qrUser/regenTempUser/%d"

module Spikes =
    let strPassTest:StrPath = "/spikes/test/%s"
    let strPassTest2:IntStrPath = "/spikes/test2/%d/%s"