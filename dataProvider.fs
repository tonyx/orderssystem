module OrdersSystem.DataProvider
open FSharp.Data
open FSharp.Configuration


type Settings = AppSettings<"App.config">


[<Literal>]
let Localization = """<xs:schema attributeFormDefault="unqualified" elementFormDefault="qualified" xmlns:xs="http://www.w3.org/2001/XMLSchema">
      <xs:element name="localization">
        <xs:complexType>
          <xs:sequence>
            <xs:element type="xs:string" name="modifyUser"/>
            <xs:element type="xs:string" name="userEnabled"/>
            <xs:element type="xs:string" name="userCanVoidOrder"/>
            <xs:element type="xs:string" name="userCanSeeAnyOrder"/>
            <xs:element type="xs:string" name="userCanChangePrices"/>
            <xs:element type="xs:string" name="userCanManageCourses"/>
            <xs:element type="xs:string" name="editCategory"/>
            <xs:element type="xs:string" name="course"/>
            <xs:element type="xs:string" name="name"/>
            <xs:element type="xs:string" name="saveChanges"/>
            <xs:element type="xs:string" name="goBack"/>
            <xs:element type="xs:string" name="addIngredientTo"/>
            <xs:element type="xs:string" name="ingredient"/>
            <xs:element type="xs:string" name="nameBySelection"/>
            <xs:element type="xs:string" name="nameByFreeText"/>
            <xs:element type="xs:string" name="quantity"/>
            <xs:element type="xs:string" name="add"/>
            <xs:element type="xs:string" name="info"/>
            <xs:element type="xs:string" name="collapseOrderItems"/>
            <xs:element type="xs:string" name="price"/>
            <xs:element type="xs:string" name="description"/>
            <xs:element type="xs:string" name="visibility"/>
            <xs:element type="xs:string" name="category"/>
            <xs:element type="xs:string" name="saveChanges"/>
            <xs:element type="xs:string" name="existingIngredients"/>
            <xs:element type="xs:string" name="categoriesOfIngredientsYouCanAdd"/>
            <xs:element type="xs:string" name="addAmong"/>
            <xs:element type="xs:string" name="addAmongAll"/>
            <xs:element type="xs:string" name="mainPage"/>
            <xs:element type="xs:string" name="changePasswordFor"/>
            <xs:element type="xs:string" name="changePassword"/>
            <xs:element type="xs:string" name="oldPassword"/>
            <xs:element type="xs:string" name="confirmPassword"/>
            <xs:element type="xs:string" name="createNewAccount"/>
            <xs:element type="xs:string" name="fillData"/>
            <xs:element type="xs:string" name="role"/>
            <xs:element type="xs:string" name="canSeeAllOrders"/>
            <xs:element type="xs:string" name="canOverWritePrices"/>
            <xs:element type="xs:string" name="error"/>
            <xs:element type="xs:string" name="anErrorOccurred"/>
            <xs:element type="xs:string" name="connectedUser"/>
            <xs:element type="xs:string" name="userRole"/>
            <xs:element type="xs:string" name="courseCategoriesManagement"/>
            <xs:element type="xs:string" name="allCourses"/>
            <xs:element type="xs:string" name="manage"/>
            <xs:element type="xs:string" name="missing"/>
            <xs:element type="xs:string" name="table"/>
            <xs:element type="xs:string" name="group"/>
            <xs:element type="xs:string" name="receipt"/>
            <xs:element type="xs:string" name="reject"/>
            <xs:element type="xs:string" name="rejectOrderItem"/>
            <xs:element type="xs:string" name="motivation"/>
            <xs:element type="xs:string" name="submit"/>
            <xs:element type="xs:string" name="actionableStatesForUser"/>
            <xs:element type="xs:string" name="disable"/>
            <xs:element type="xs:string" name="enable"/>
            <xs:element type="xs:string" name="actionableStatesForWaiters"/>
            <xs:element type="xs:string" name="actionableStatesForTemporaryUsers"/>
            <xs:element type="xs:string" name="variation"/>
            <xs:element type="xs:string" name="total"/>
            <xs:element type="xs:string" name="variationOverPriceDiscount"/>
            <xs:element type="xs:string" name="value"/>
            <xs:element type="xs:string" name="percentage"/>
            <xs:element type="xs:string" name="voidDiscounts"/>
            <xs:element type="xs:string" name="end"/>
            <xs:element type="xs:string" name="ingredients"/>
            <xs:element type="xs:string" name="confirmGroup"/>
            <xs:element type="xs:string" name="reprintGroup"/>
            <xs:element type="xs:string" name="newOrder"/>
            <xs:element type="xs:string" name="order"/>
            <xs:element type="xs:string" name="unArchiveLatest"/>
            <xs:element type="xs:string" name="totalDiscounted"/>
            <xs:element type="xs:string" name="archive"/>
            <xs:element type="xs:string" name="modify"/>
            <xs:element type="xs:string" name="subdivide"/>
            <xs:element type="xs:string" name="receipt"/>
            <xs:element type="xs:string" name="invoice"/>
            <xs:element type="xs:string" name="all"/>
            <xs:element type="xs:string" name="variations"/>
            <xs:element type="xs:string" name="ingredientToAdd"/>
            <xs:element type="xs:string" name="courseBySelection"/>
            <xs:element type="xs:string" name="courseByFreeText"/>
            <xs:element type="xs:string" name="exitGroup"/>
            <xs:element type="xs:string" name="comment"/>
            <xs:element type="xs:string" name="person"/>
            <xs:element type="xs:string" name="addOrder"/>
            <xs:element type="xs:string" name="changeTo"/>
            <xs:element type="xs:string" name="update"/>
            <xs:element type="xs:string" name="newCategory"/>
            <xs:element type="xs:string" name="createNewCourse"/>
            <xs:element type="xs:string" name="manageCategories"/>
            <xs:element type="xs:string" name="addCategory"/>
            <xs:element type="xs:string" name="storedCategories"/>
            <xs:element type="xs:string" name="manageCourses"/>
            <xs:element type="xs:string" name="storedCourses"/>
            <xs:element type="xs:string" name="manageVisibleCoursesOfCategory"/>
            <xs:element type="xs:string" name="seeAll"/>
            <xs:element type="xs:string" name="addNew"/>
            <xs:element type="xs:string" name="visibleCoursesOfCategory"/>
            <xs:element type="xs:string" name="clickToChange"/>
            <xs:element type="xs:string" name="allTheItems"/>
            <xs:element type="xs:string" name="visibleOfCategory"/>
            <xs:element type="xs:string" name="addNewItem"/>
            <xs:element type="xs:string" name="coursesOfCategory"/>
            <xs:element type="xs:string" name="allVisibleCoursesOfCategory"/>
            <xs:element type="xs:string" name="clickToSeeAllVisibleCoursesOfCategory"/>
            <xs:element type="xs:string" name="clickToSeeAllCoursesOfCategory"/>
            <xs:element type="xs:string" name="addQuantityIn"/>
            <xs:element type="xs:string" name="modifyIngredient"/>
            <xs:element type="xs:string" name="allergen"/>
            <xs:element type="xs:string" name="updateAvailability"/>
            <xs:element type="xs:string" name="checkAvailability"/>
            <xs:element type="xs:string" name="measuringSystem"/>
            <xs:element type="xs:string" name="all"/>
            <xs:element type="xs:string" name="newIngredient"/>
            <xs:element type="xs:string" name="availablQuantity"/>
            <xs:element type="xs:string" name="visibleCategoriesOfIngredients"/>
            <xs:element type="xs:string" name="createNewCategory"/>
            <xs:element type="xs:string" name="visibleExistingCategories"/>
            <xs:element type="xs:string" name="managePrinters"/>
            <xs:element type="xs:string" name="deletePrinters"/>
            <xs:element type="xs:string" name="detectPrinters"/>
            <xs:element type="xs:string" name="allCategoriesOfIngredients"/>
            <xs:element type="xs:string" name="existingCategories"/>
            <xs:element type="xs:string" name="temporaryUser"/>
            <xs:element type="xs:string" name="nameOrTable"/>
            <xs:element type="xs:string" name="manageTemporaryUsers"/>
            <xs:element type="xs:string" name="createTemporaryUsers"/>
            <xs:element type="xs:string" name="manageUsers"/>
            <xs:element type="xs:string" name="addUser"/>
            <xs:element type="xs:string" name="linkStates"/>
            <xs:element type="xs:string" name="regenerateTempUser"/>
            <xs:element type="xs:string" name="userExpired"/>
            <xs:element type="xs:string" name="theName"/>
            <xs:element type="xs:string" name="deletionPage"/>
            <xs:element type="xs:string" name="warningDeletion"/>
            <xs:element type="xs:string" name="deleteUsers"/>
            <xs:element type="xs:string" name="deleteTemporaryUsers"/>
            <xs:element type="xs:string" name="deleteCourses"/>
            <xs:element type="xs:string" name="deleteCourseCategories"/>
            <xs:element type="xs:string" name="deleteRoles"/>
            <xs:element type="xs:string" name="deleteIngredients"/>
            <xs:element type="xs:string" name="deleteIngredientCategories"/>
            <xs:element type="xs:string" name="delete"/>
            <xs:element type="xs:string" name="remove"/>
            <xs:element type="xs:string" name="roleName"/>
            <xs:element type="xs:string" name="createRole"/>
            <xs:element type="xs:string" name="allTheOrders"/>
            <xs:element type="xs:string" name="courseCategories"/>
            <xs:element type="xs:string" name="manageRoles"/>
            <xs:element type="xs:string" name="manageIngredients"/>
            <xs:element type="xs:string" name="manageOrdinaryUsers"/>
            <xs:element type="xs:string" name="managePayments"/>
            <xs:element type="xs:string" name="addIngredient"/>
            <xs:element type="xs:string" name="originalPrice"/>
            <xs:element type="xs:string" name="recalculatedPrice"/>
            <xs:element type="xs:string" name="existingVariations"/>
            <xs:element type="xs:string" name="void"/>
            <xs:element type="xs:string" name="possibleVariations"/>
            <xs:element type="xs:string" name="confirm"/>
            <xs:element type="xs:string" name="voidOrder"/>
            <xs:element type="xs:string" name="activeOrders"/>
            <xs:element type="xs:string" name="seeIngredientUsages"/>
            <xs:element type="xs:string" name="ordinaryQuantity"/>
            <xs:element type="xs:string" name="seeComsumptionStartingFrom"/>
            <xs:element type="xs:string" name="startingFrom"/>
            <xs:element type="xs:string" name="visualize"/>
            <xs:element type="xs:string" name="priceForIngredient"/>
            <xs:element type="xs:string" name="addPrice"/>
            <xs:element type="xs:string" name="subtractPrice"/>
            <xs:element type="xs:string" name="isDefaultAddQuantity"/>
            <xs:element type="xs:string" name="isDefaultSubtractQuantity"/>
            <xs:element type="xs:string" name="allIngredientsOfCategory"/>
            <xs:element type="xs:string" name="searchByName"/>
            <xs:element type="xs:string" name="search"/>
            <xs:element type="xs:string" name="viewUsages"/>
            <xs:element type="xs:string" name="load"/>
            <xs:element type="xs:string" name="addIngredientPrice"/>
            <xs:element type="xs:string" name="button"/>
            <xs:element type="xs:string" name="sixToTwentyChars"/>
            <xs:element type="xs:string" name="abstract"/>
            <xs:element type="xs:string" name="changeVisibility"/>
            <xs:element type="xs:string" name="seeOnlyVisibles"/>
            <xs:element type="xs:string" name="userName"/>
            <xs:element type="xs:string" name="defaultStatesForWaiter"/>
            <xs:element type="xs:string" name="defaultStatesForTemporaryUsers"/>
            <xs:element type="xs:string" name="listOfExistingRoles"/>
            <xs:element type="xs:string" name="enableState"/>
            <xs:element type="xs:string" name="viewStates"/>
            <xs:element type="xs:string" name="accessRights"/>
            <xs:element type="xs:string" name="optimization"/>
            <xs:element type="xs:string" name="deletions"/>
            <xs:element type="xs:string" name="orders"/>
            <xs:element type="xs:string" name="logoff"/>
            <xs:element type="xs:string" name="orderItemStates"/>
            <xs:element type="xs:string" name="yes"/>
            <xs:element type="xs:string" name="no"/>
            <xs:element type="xs:string" name="isDefaultAddAndSubtractQuantity"/>
            <xs:element type="xs:string" name="addRole"/>
            <xs:element type="xs:string" name="merge"/>
            <xs:element type="xs:string" name="rollBackLatestVoided"/>
            <xs:element type="xs:string" name="allTheOrders"/>
            <xs:element type="xs:string" name="myOwn"/>
            <xs:element type="xs:string" name="ofOthers"/>
            <xs:element type="xs:string" name="coursesOfType"/>
            <xs:element type="xs:string" name="find"/>
          </xs:sequence>
        </xs:complexType>
      </xs:element>
    </xs:schema>"""



type Resource = XmlProvider<Schema=Localization>








