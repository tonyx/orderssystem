
alignCheckBoxes();

document.getElementById("categoriesids").addEventListener("change", alignCheckBoxes);
document.getElementById("rolesids").addEventListener("change", alignCheckBoxes);


function alignCheckBoxes() {
    var index1 = document.getElementById("rolesids").value;
    var index2 = document.getElementById("categoriesids").value;
    var compositeIndex = index1+","+index2;
    var enablers = (rolescategoriesenablers.get(compositeIndex));
    var observers = (rolescategoriesobservers.get(compositeIndex));

    // var rolescatKeys = rolescategoriesenablers.keys();
    // var observersKeys = rolescategoriesobservers.keys();


    // for (var key in rolescatKeys ) {
    // 	alert(key);
    // }

    // for (var key in observersKeys ) {
    // 	alert(key);
    // }

    for (i=0;i<allCheckBoxes.length;i++) {
        document.getElementsByName(allCheckBoxes[i])[0].checked = false;
    }

    for (i=0; i< enablers.length;i++) {
        document.getElementsByName(enablers[i])[0].checked=true;
    }
    for (i=0; i< observers.length;i++) {
        document.getElementsByName(observers[i])[0].checked=true;
    }
}


function handleChange(checkbox) {
	var name = checkbox.getAttribute("name");
	if (checkbox.checked == true) {
		if (name.startsWith("Enabler")) {
			var propagateTo = "Observer"+name.substring(7);
			document.getElementsByName(propagateTo)[0].checked=true;
		}
	}
	else  {
		if (name.startsWith("Observer")) {
			var propagateTo = "Enabler"+name.substring(8);
			document.getElementsByName(propagateTo)[0].checked=false;
		}
	}
}