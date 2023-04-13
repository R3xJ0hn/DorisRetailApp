window.registerSidebarToggleHandler = function () {
    const sidebarToggleBtn = document.getElementById('sidebar-toggle-btn');
    const sidebarToggleXBtn = document.getElementById('sidebar-toggle-x-btn');

    const sidebarToggleBackdrop = document.querySelector('.sidebar-backdrop');

    const sidebar = window.parent.document.getElementById('sidebar');

    sidebarToggleBtn.addEventListener('click', function (event) {
        toggleSidebar();
    });

    sidebarToggleXBtn.addEventListener('click', function (event) {
        toggleSidebar();
    });

    sidebarToggleBackdrop.addEventListener('click', function (event) {
        toggleSidebar();
    });

    function toggleSidebar() {
        sidebar.classList.toggle('collapsed');
    }

};

function OpenPanel1() {
    const panel1 = document.getElementById('panel1');
    const panel2 = document.getElementById('panel2');
    handleClick(panel1, panel2);
}

function OpenPanel2() {
    const panel1 = document.getElementById('panel1');
    const panel2 = document.getElementById('panel2');
    handleClick(panel2, panel1);
}

function handleClick(panelToShow, panelToHide) {
    let timeoutId = null;
    const btn1 = document.getElementById('openPanel1Btn');
    const btn2 = document.getElementById('openPanel2Btn');

    if (timeoutId !== null) {
        btn1.preventDefault();
        btn2.preventDefault();
        return false;
    }

    let appContainer = document.querySelector(".app-container");
    appContainer.style.height = '150vh';
    window.scrollTo(0, 0);

    panelToHide.classList.remove('in');
    panelToHide.classList.add('out');
    panelToShow.classList.remove('out');
    panelToShow.classList.add('in');

    panelToShow.style.display = 'flex';
    panelToShow.style.opacity = '1';

    timeoutId = setTimeout(function () {
        panelToHide.style.display = 'none';
        appContainer.style.height = 'auto';
        timeoutId = null;
    }, 500);
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

function uploadImage() {
    const selectImgBtn = document.getElementById("open-img");
    selectImgBtn.click();

    selectImgBtn.addEventListener("change", function () {
        const wrapper = document.querySelector(".upload-image-wrapper");
        const img = document.querySelector(".upload-image");
        const fileName = document.querySelector(".file-name");
        const cancelBtn = document.querySelector("#cancel-btn i");

        let regExp = /[0-9a-zA-Z\^\&\'\@\{\}\[\]\,\$\=\!\-\#\(\)\.\%\+\~\_ ]+$/;

        const file = this.files[0];
        if (file) {
            const reader = new FileReader();
            reader.onload = function () {
                const result = reader.result;
                img.src = result;
                wrapper.classList.add("active");
            }

            cancelBtn.addEventListener("click", function () {
                clearUploadValue();
            })

            reader.readAsDataURL(file);
        }
        if (this.value) {
            let valueStore = this.value.match(regExp);
            fileName.textContent = valueStore;
        }
    });
}

function setUploadImageToDeactive() {
    const wrapper = document.querySelector(".upload-image-wrapper");
    wrapper.classList.remove("active");
}

function setUploadImageToActive(url) {
    const img = document.querySelector(".upload-image");
    const wrapper = document.querySelector(".upload-image-wrapper");
    wrapper.classList.add("active");
    img.src = url;
}

function clearUploadValue() {
    const fileInput = document.getElementById("open-img");
    const wrapper = document.querySelector(".upload-image-wrapper");
    const img = document.querySelector(".upload-image");

    if (fileInput.value !== "") {
        fileInput.value = "";
        img.src = "";
        wrapper.classList.remove("active");
    }

}

function SavedAlert(type, name) {
    Swal.fire(
        'Successfully saved new ' + type,
        'You added '+ name + ' on the list!',
        'success'
    )
}

function ExistAlert(type) {
    Swal.fire({
        title: type + ' Exist!',
        icon: 'warning'
    })
}

function UnauthorizedAlert() {
    Swal.fire({
        title: 'Unauthorized',
        icon: 'error'
    })
}

function ServerErrorAlert(){
    Swal.fire({
        title: 'Server Error',
        icon: 'error'
    })
}

const Toast = Swal.mixin({
    toast: true,
    position: 'top-right',
    iconColor: 'white',
    customClass: {
        popup: 'colored-toast'
    },
    showConfirmButton: false,
    timer: 1500,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer)
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
})

const ToastSuccess = Swal.mixin({
    toast: true,
    position: 'top-end',
    showConfirmButton: false,
    timer: 3000,
    timerProgressBar: true,
    didOpen: (toast) => {
        toast.addEventListener('mouseenter', Swal.stopTimer)
        toast.addEventListener('mouseleave', Swal.resumeTimer)
    }
})

function SuccessToast(msg) {
    ToastSuccess.fire({
        icon: 'success',
        title: msg
    })
}

function ErrorToast(msg) {
    Toast.fire({
        icon: 'error',
        title: msg
    })
}

function WarningToast(msg) {
    Toast.fire({
        icon: 'warning',
        title: msg
    })
}

function SetDatePickerSetting(){
    $('.date-picker .form-control').datepicker({
        format: 'M d, yyyy',
        orientation: "bottom left",
        autoclose: true,
        todayHighlight: true
    });
}

function DateChange(ref) {
    $('.date-picker .form-control').on('changeDate', function () {
        ref.invokeMethodAsync('HandleChangeDateEvent');
    });
}

function getDateValue(inputId) {
    var input = document.getElementById(inputId);
    input.addEventListener("input", () => { startTimer(1); }, { once: true });
    return input.value;

}

function InventoryCarouselNext() {
    const myCarousel = document.getElementById('InventorySlide')
    const carousel = new bootstrap.Carousel(myCarousel)
    carousel.next()
    carousel.dispose()
}

function ShowPasswordPopup(modal) {

    if (modal !== null) {
        $(modal).on('shown.bs.modal', function (e) {
            $(document).off('focusin.modal');
        })
    }

    var passwordPopUp = document.querySelector(".password-popup");
    passwordPopUp.style.display = "block";
    document.querySelector(".password-popup-backdrop").style.display = "block";
}

function ClosePasswordPopup() {
    var passwordPopUp = document.querySelector(".password-popup");
    passwordPopUp.style.display = "none";
    document.querySelector(".password-popup-backdrop").style.display = "none";
}

var stamp = "";

function ConfirmIdentity(ref) {
    $('#securityStamp').on('click', function () {
        ref.invokeMethodAsync('HandleConfirmedIdentity', stamp);
    });
}

function StampChange(str) {
    stamp = str;
    $('#securityStamp').click();
}

function scrollToListBottom(scollableListId) {
    var element = document.querySelector(scollableListId);
    element.scrollTop = element.scrollHeight;
}

function ImportCategoryCarouselNext() {
    const myCarousel = document.getElementById('importCategorySlide')
    const carousel = new bootstrap.Carousel(myCarousel)
    carousel.next()
    carousel.dispose()
}

function ImportCategoryCarouselReset() {
    var carouselElement = document.getElementById('importCategorySlide');
    var carousel = new bootstrap.Carousel(carouselElement, {
        interval: false 
    });

    carousel.to(0);
    carousel.dispose()

}