document.addEventListener("DOMContentLoaded", function () {
    document.getElementById("resetPasswordForm").addEventListener("submit", function (event) {
        var password = document.getElementById("Password").value;
        var passwordHelp = document.getElementById("passwordHelp");
        var regex = /^(?=.*[0-9])(?=.*[!@#$%^&*])[a-zA-Z0-9!@#$%^&*]{8,}$/;

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