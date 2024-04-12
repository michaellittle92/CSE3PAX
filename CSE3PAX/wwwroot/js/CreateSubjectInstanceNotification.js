document.addEventListener('DOMContentLoaded', function () {
    const selectButtons = document.querySelectorAll('.select-lecturer');

    selectButtons.forEach(button => {
        button.addEventListener('click', function () {
            const email = this.getAttribute('data-email');
            const firstname = this.getAttribute('data-firstname');
            const lastname = this.getAttribute('data-lastname');
            const loadCapacity = parseFloat(this.getAttribute('data-load-capacity'));

            // Check if the load capacity is above 100%
            if (loadCapacity > 100) {
                // Show warning message
                Swal.fire({
                    icon: 'warning',
                    //title: 'Warning',
                    html: `<span style="font-size: 1.1em;">${firstname} ${lastname} is currently overcommitted (${loadCapacity}%).</span>`,
                    confirmButtonColor: '#0D6EFD',
                    confirmButtonText: 'OK'
                });
            } else {
            }
        });
    });
});