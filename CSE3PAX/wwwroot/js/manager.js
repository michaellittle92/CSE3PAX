document.addEventListener('DOMContentLoaded', function () {
    // Get the dropdown button element
    var dropdownButton = document.getElementById('lecturerDropdownButton');

    // Get the dropdown menu element
    var dropdownMenu = document.querySelector('.dropdown-menu');

    // Add event listener to the dropdown button
    dropdownButton.addEventListener('click', function (event) {
        // Toggle the 'show' class on the dropdown menu
        dropdownMenu.classList.toggle('show');
        // Prevent the click event from propagating to the document click listener
        event.stopPropagation();
    });

    // Add event listener to the document to hide the dropdown menu when clicking outside of it
    document.addEventListener('click', function (event) {
        // Check if the clicked element is not the dropdown button or menu
        if (!dropdownButton.contains(event.target) && !dropdownMenu.contains(event.target)) {
            // Hide the dropdown menu
            dropdownMenu.classList.remove('show');
        }
    });
});

