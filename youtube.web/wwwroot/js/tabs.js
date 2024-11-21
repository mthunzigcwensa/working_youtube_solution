// JavaScript for tab switching functionality
document.querySelectorAll('.tab').forEach(tab => {
    tab.addEventListener('click', function () {
        // Remove active class from all tabs
        document.querySelectorAll('.tab').forEach(item => item.classList.remove('active'));

        // Add active class to the clicked tab
        this.classList.add('active');

        // Hide all tab contents
        document.querySelectorAll('.tab-content').forEach(content => content.classList.remove('active'));

        // Show the content for the clicked tab
        document.getElementById(this.getAttribute('data-tab')).classList.add('active');
    });
});
