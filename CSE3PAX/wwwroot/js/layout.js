/*
JavaScript file for the _Layout page to handle the active navigation
of links when clicked. Dashboard is set by default.
*/

// Function to handle link clicks
function handleLinkClick(link) {
    // Remove active class from all links
    document.querySelectorAll('.list-group-item').forEach(item => item.classList.remove('active'));
    // Add active class to the clicked link
    link.classList.add('active');
    // Store the href of the clicked link in local storage
    localStorage.setItem('activeLink', link.getAttribute('href'));
}

// Execute when the DOM content is loaded
document.addEventListener('DOMContentLoaded', () => {
    // Get the active link from local storage on page load
    const activeLink = localStorage.getItem('activeLink');
    const defaultLink = '/Shared/Dashboard';
    const defaultActiveLink = document.querySelector(`.list-group-item[href="${defaultLink}"]`);

    // Set active link based on stored value or default to dashboard
    if (activeLink && activeLink !== '/Shared/Logout') {
        const storedActiveLink = document.querySelector(`.list-group-item[href="${activeLink}"]`);
        if (storedActiveLink) storedActiveLink.classList.add('active');
        else defaultActiveLink.classList.add('active');
    } else {
        defaultActiveLink.classList.add('active');
        localStorage.setItem('activeLink', defaultLink);
    }

    // Toggle profile menu visibility
    const profileMenuButton = document.getElementById('profileMenuButton');
    const profileMenu = document.getElementById('profileMenu');

    profileMenuButton.addEventListener('click', event => {
        event.preventDefault();
        profileMenu.style.display = profileMenu.style.display === 'none' ? 'block' : 'none';
    });
});

// Get all links inside the list group
const links = document.querySelectorAll('.list-group-item');

// Add click event listener to each link
links.forEach(link => {
    link.addEventListener('click', event => {
        // Prevent the default action
        event.preventDefault();

        // Check if the clicked link is different from the current URL
        if (window.location.href !== link.href) {
            // Handle link click
            handleLinkClick(link);

            // Get the href attribute of the clicked link
            const href = link.getAttribute('href');

            // Navigate to the specified URL after a short delay
            setTimeout(() => { window.location.href = href; }, 100);
        }
    });
});