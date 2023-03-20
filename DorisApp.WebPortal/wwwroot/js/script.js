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

openPanelBtn1.addEventListener('click', function (event) {
    handleClick(openPanelBtn1, event, panel1, panel2);
});

openPanelBtn2.addEventListener('click', function (event) {
    handleClick(openPanelBtn2, event, panel2, panel1);
});


const panel1 = document.getElementById('panel1');
const panel2 = document.getElementById('panel2');
let timeoutId = null;

function handleClick(event, panelToShow, panelToHide) {
    if (timeoutId !== null) {
        event.preventDefault();
        return false;
    }

    panelToHide.classList.remove('in');
    panelToHide.classList.add('out');
    panelToShow.classList.remove('out');
    panelToShow.classList.add('in');

    panelToShow.style.display = 'flex';
    panelToShow.style.opacity = '1';

    // Start the timeout
    timeoutId = setTimeout(function () {
        panelToHide.style.display = 'none';
        timeoutId = null;
    }, 300);
}





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
    enableButtons();

    if (typeof (modal) != 'undefined' && modal != null) {
        modal.hide();
    }
}

function enableButtons() {
    var proceedBtn = document.getElementById('saveButton');
    var buttons = document.querySelectorAll(".btn");
    for (var i = 0; i < buttons.length; i++) {
        buttons[i].classList.remove("disabled");
    }
    proceedBtn.querySelector(".spinner-border").classList.add("d-none");
}

function showToast(toastId) {
    var toast = new bootstrap.Toast(document.querySelector(toastId))
    toast.show()
}
