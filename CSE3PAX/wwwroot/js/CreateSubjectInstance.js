/* 
 Select all elements with the class 'select lecturer' and attack a click listener to each.
 When a button is clicked, the event listener executes a function that retrieves the data attributes.
*/

// Select all elements with the class 'select-lecturer' and attach a click event listener to each
document.querySelectorAll('.select-lecturer').forEach(button => {
    button.addEventListener('click', function () {

        // Retrieve data attributes from the clicked button
        const email = this.getAttribute('data-email');
        const firstName = this.getAttribute('data-firstname');
        const lastName = this.getAttribute('data-lastname');

        // Update the readonly input fields
        document.getElementById('selectedEmail').value = email;
        document.getElementById('selectedFirstName').value = firstName;
        document.getElementById('selectedLastName').value = lastName;

        // Set the value of a hidden input field 'SelectedEmail' if it exists.
        const hiddenEmailInput = document.querySelector('input[name="SelectedEmail"]');
        if (hiddenEmailInput) {
            hiddenEmailInput.value = email;
        }
    });
});
