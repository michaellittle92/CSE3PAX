/*
 Retrieves references to checkbox elements and div container from the DOM.
 DOM is initially hidden when the lecturer checkbox is changed, it unchecks the manager and admin checkboxes and displays the lecturer fields. 
 */

// Retrieve references to checkbox elements and div containiner
const lecturerCheckBox = document.getElementById("IsLecturer");
const managerCheckBox = document.getElementById("IsManager");
const adminCheckBox = document.getElementById("IsAdmin");
const lecturerFieldsDiv = document.getElementById("lecturerFieldsDiv");
const lecturerFields = document.getElementsByClassName("requiredWhenVisible");

// Hide the div container for lecturer fields
lecturerFieldsDiv.style.display = "none";

// Event listener for lecturer checkbox
lecturerCheckBox.addEventListener('change', function () {
    if (this.checked) {
        // If lecturerCheckBox is checked, uncheck managerCheckBox and adminCheckBox set lecturer feilds to display
        managerCheckBox.checked = false;
        adminCheckBox.checked = false;
        lecturerFieldsDiv.style.display = "block";
    }
});

// Event listener for manager checkbox
managerCheckBox.addEventListener('change', function () {
    if (this.checked) {
        // If managerCheckBox is checked, uncheck lecturerCheckBox
        lecturerCheckBox.checked = false;
        adminCheckBox.checked = false;
        lecturerFieldsDiv.style.display = "none";
    }
});

// Event listener for admin checkbox
adminCheckBox.addEventListener('change', function () {
    if (this.checked) {
        // If managerCheckBox is checked, uncheck lecturerCheckBox
        managerCheckBox.checked = false;
        lecturerCheckBox.checked = false;
        lecturerFieldsDiv.style.display = "none";
    }
});
