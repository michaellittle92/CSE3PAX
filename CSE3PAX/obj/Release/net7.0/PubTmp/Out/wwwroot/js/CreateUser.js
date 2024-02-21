const lecturerCheckBox = document.getElementById("IsLecturer");
const managerCheckBox = document.getElementById("IsManager");
const adminCheckBox = document.getElementById("IsAdmin");
const lecturerFieldsDiv = document.getElementById("lecturerFieldsDiv");
const lecturerFields = document.getElementsByClassName("requiredWhenVisible");

lecturerFieldsDiv.style.display = "none";


lecturerCheckBox.addEventListener('change', function () {
    if (this.checked) {
        // If lecturerCheckBox is checked, uncheck managerCheckBox and adminCheckBox set lecturer feilds to display
        managerCheckBox.checked = false;
        adminCheckBox.checked = false;
        lecturerFieldsDiv.style.display = "block";
    }
});

managerCheckBox.addEventListener('change', function () {
    if (this.checked) {
        // If managerCheckBox is checked, uncheck lecturerCheckBox
        lecturerCheckBox.checked = false;
        adminCheckBox.checked = false;
        lecturerFieldsDiv.style.display = "none";


    }
});
adminCheckBox.addEventListener('change', function () {
    if (this.checked) {
        // If managerCheckBox is checked, uncheck lecturerCheckBox
        managerCheckBox.checked = false;
        lecturerCheckBox.checked = false;
        lecturerFieldsDiv.style.display = "none";

    }
});
