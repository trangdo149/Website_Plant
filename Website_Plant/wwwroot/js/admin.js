//// Lấy tất cả các liên kết trong menu
//const menuLinks = document.querySelectorAll('.menu-list .menu-item a');

//// Lặp qua từng liên kết
//menuLinks.forEach(link => {
//    link.addEventListener('click', function () {
//        // Xóa class menu-item--active khỏi tất cả các mục menu
//        document.querySelectorAll('.menu-item').forEach(item => {
//            item.classList.remove('menu-item--active');
//        });

//        // Thêm class menu-item--active vào mục menu được nhấn
//        this.parentElement.classList.add('menu-item--active');
//    });
//});

//// Ghi nhớ trạng thái khi tải lại trang
//window.addEventListener('DOMContentLoaded', () => {
//    const currentUrl = window.location.pathname;
//    menuLinks.forEach(link => {
//        if (link.getAttribute('href') === currentUrl) {
//            link.parentElement.classList.add('menu-item--active');
//        }
//    });
//});

// Lấy tất cả các liên kết trong menu
const menuLinks = document.querySelectorAll('.menu-list .menu-item a');

// Lặp qua từng liên kết
menuLinks.forEach(link => {
    link.addEventListener('click', function () {
        // Xóa class menu-item--active khỏi tất cả các mục menu
        document.querySelectorAll('.menu-item').forEach(item => {
            item.classList.remove('menu-item--active');
        });

        // Thêm class menu-item--active vào mục menu được nhấn
        this.parentElement.classList.add('menu-item--active');
    });
});

// Ghi nhớ trạng thái khi tải lại trang
window.addEventListener('DOMContentLoaded', () => {
    const currentUrl = window.location.pathname; // Lấy đường dẫn hiện tại
    let isActiveSet = false; // Kiểm tra nếu đã đặt trạng thái active

    document.querySelectorAll('.menu-item').forEach(item => {
        item.classList.remove('menu-item--active'); // Đảm bảo xóa trạng thái cũ
    });

    menuLinks.forEach(link => {
        if (link.getAttribute('href') === currentUrl) {
            link.parentElement.classList.add('menu-item--active');
            isActiveSet = true; // Đã tìm thấy tab khớp URL
        }
    });

    // Nếu không có tab nào được active, đặt mặc định vào tab Quản lý sản phẩm
    if (!isActiveSet) {
        const defaultTab = document.querySelector('.menu-list .menu-item:first-child');
        if (defaultTab) {
            defaultTab.classList.add('menu-item--active');
        }
    }
});

const fileInput = document.getElementById('fileUpload');
const fileName = document.querySelector('.file-name');

// Hiển thị tên file khi người dùng chọn tệp
fileInput.addEventListener('change', function () {
    if (fileInput.files.length > 0) {
        fileName.textContent = fileInput.files[0].name; // Lấy tên tệp đã chọn
    } else {
        fileName.textContent = 'Chưa chọn tệp'; // Khi không có tệp nào
    }
});

