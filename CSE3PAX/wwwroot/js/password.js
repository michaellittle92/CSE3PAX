/*
 When the form is submitted, the code retrieves the value of the password input field and verifies it against a regular expression
 pattern that enforces specific password criteria: the password must be at least 8 characters long and contain at least one number
 and one special character.

 If the entered password does not meet the required format, the form submission is prevented, and an error message is displayed
 to the user, indicating the password requirements. If the password meets the criteria, any existing error message is cleared,
 and the form submission proceeds as normal.
*/


// Execute when the DOM content is loaded
document.addEventListener("DOMContentLoaded", function () {

    // Add event listener to the password reset for submission
    document.getElementById("resetPasswordForm").addEventListener("submit", function (event) {
        // Retrieve the value of the password input field
        var password = document.getElementById("Password").value;

        // Retrieve the element for displaying password help/error messages
        var passwordHelp = document.getElementById("passwordHelp");

        // Regular expression to validate password format
        var regex = /^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{8,}$/;

        // Check if the password matches the regular expression
        if (!regex.test(password)) {
            // Prevent form submission
            event.preventDefault();
            // Show error message
            passwordHelp.textContent = "Password must be at least 8 characters long and include at least one number and one special character.";
        } else {
            // Clear any error message
            passwordHelp.textContent = "";
        }
    });
});