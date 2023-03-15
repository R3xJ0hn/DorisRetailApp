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


function modalProcessing() {
    var proceedBtn = document.getElementById('saveButton');
    var buttons = document.querySelectorAll(".btn");
    for (var i = 0; i < buttons.length; i++) {
        buttons[i].classList.add("disabled");
    }
    proceedBtn.querySelector(".spinner-border").classList.remove("d-none");
}

function modalProcessDone(modalId) {
    const myModalEl = document.querySelector(modalId);
    var modal = bootstrap.Modal.getInstance(myModalEl)

    var proceedBtn = document.getElementById('saveButton');
    var buttons = document.querySelectorAll(".btn");
    for (var i = 0; i < buttons.length; i++) {
        buttons[i].classList.remove("disabled");
    }
    proceedBtn.querySelector(".spinner-border").classList.add("d-none");

    if (typeof (modal) != 'undefined' && modal != null) {
        modal.hide();
    }
}

function showToast(toastId) {
    var toast = new bootstrap.Toast(document.querySelector(toastId))
    toast.show()
}
