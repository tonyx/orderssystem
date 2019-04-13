
document.getElementsByName("CategoryId")[0].addEventListener("change",fillCheckBoxes);
fillCheckBoxes();

function fillCheckBoxes() {
	let category = document.getElementsByName("CategoryId")[0].value;

	for (i=0;i<allCheckBoxesNames.length;i++) {
		document.getElementsByName(allCheckBoxesNames[i])[0].checked = false;
	}

	let enabledStates = enabledStatesforcategories.get(category);
	for (i=0;i<enabledStates.length;i++) {
		document.getElementsByName(enabledStates[i])[0].checked=true;
	}
}
