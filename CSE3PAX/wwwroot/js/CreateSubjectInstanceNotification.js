/*
 Add event listen to DOMContentLoaded event.
 Select all elements with the class 'select lecturer' and attach a click event listener to each.
 Within the click event handler, the code checks if the load capacity exceeds 100%. If it does, it displays a warning message using the 
 SweetAlert2 library, indicating that the lecturer is currently overcommitted. The warning message includes the lecturer's full name 
 and the percentage of load capacity exceeding 100%. If the load capacity is not above 100%, no action is taken.
*/

// Add an event listener to execute when the DOM is loaded
document.addEventListener('DOMContentLoaded', function () {

    // Select all elements with the class 'select-lecturer'
    const selectButtons = document.querySelectorAll('.select-lecturer');

    // Iterate over each 'select-lecturer' element
    selectButtons.forEach(button => {
        button.addEventListener('click', function () {

            // Retrieve data attributes from the clicked button
            const email = this.getAttribute('data-email');
            const firstname = this.getAttribute('data-firstname');
            const lastname = this.getAttribute('data-lastname');
            const loadCapacity = parseFloat(this.getAttribute('data-load-capacity'));

            // Check if the load capacity is above 100%
            if (loadCapacity > 100) {
                // If load capacity exceeds 100%, display warning message
                Swal.fire({
                    icon: 'warning',
                    //title: 'Warning',
                    // Message to show the lecturers name and their overall load capacity
                    html: `<span style="font-size: 1.1em;">${firstname} ${lastname} is currently overcommitted (${loadCapacity}%).</span>`,
                    confirmButtonColor: '#0D6EFD',
                    confirmButtonText: 'OK'
                });
            } else {
                // If load capacity is within limits, no action is taken
            }
        });
    });
});