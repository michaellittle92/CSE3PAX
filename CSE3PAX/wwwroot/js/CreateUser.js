const lecturerCheckBox = document.getElementById("IsLecturer");
const lecturerFieldsDiv = document.getElementById("lecturerFieldsDiv");
const lecturerFields = document.getElementsByClassName("requiredWhenVisible");

// Initially hide the fields
lecturerFieldsDiv.style.display = "none";

lecturerCheckBox.addEventListener('change', function () {
    if (this.checked) {
        lecturerFieldsDiv.style.display = "block";
        Array.from(lecturerFields).forEach(field => field.required = true);
    } else {
        lecturerFieldsDiv.style.display = "none";
        Array.from(lecturerFields).forEach(field => field.required = false);
    }
});