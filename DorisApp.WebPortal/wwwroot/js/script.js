window.registerSidebarToggleHandler = function () {
    const sidebarToggleBtn = document.getElementById('sidebar-toggle-btn');
    const sidebarToggleXBtn = document.getElementById('sidebar-toggle-x-btn');
    const sidebar = window.parent.document.getElementById('sidebar');

    sidebarToggleBtn.addEventListener('click', function (event) {
        toggleSidebar();
    });

    sidebarToggleXBtn.addEventListener('click', function (event) {
        toggleSidebar();
    });

    function toggleSidebar() {
        sidebar.classList.toggle('collapsed');
    }

};