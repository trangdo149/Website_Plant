// Hàm đặt trạng thái active cho menu tương ứng
function setActiveMenu(menuSelector, matchPath) {
    const menuLink = document.querySelector(menuSelector);
    if (menuLink && window.location.pathname.includes(matchPath)) {
        menuLink.parentElement.classList.add('menu-item--active');
        return true; // Trạng thái active đã được đặt
    }
    return false; // Không tìm thấy tab khớp
}

// Ghi nhớ trạng thái active khi tải lại trang
window.addEventListener('DOMContentLoaded', () => {
    // Xóa trạng thái active cũ
    document.querySelectorAll('.menu-item').forEach(item => {
        item.classList.remove('menu-item--active');
    });

    // Đặt trạng thái active cho từng tab
    let isActiveSet = false;
    isActiveSet = setActiveMenu('.menu-item a[href="/Admin/Message"]', '/Admin/Message') || isActiveSet;
    isActiveSet = setActiveMenu('.menu-item a[href="/Admin/Order"]', '/Admin/Order') || isActiveSet;
    isActiveSet = setActiveMenu('.menu-item a[href="/Admin/Plant"]', '/Admin/Plant') || isActiveSet;

    // Nếu không có tab nào được active, đặt mặc định vào tab Quản lý sản phẩm
    if (!isActiveSet) {
        const defaultTab = document.querySelector('.menu-list .menu-item:first-child');
        if (defaultTab) {
            defaultTab.classList.add('menu-item--active');
        }
    }
});

// Xử lý sự kiện click vào menu để thêm trạng thái active
const menuLinks = document.querySelectorAll('.menu-list .menu-item a');
menuLinks.forEach(link => {
    link.addEventListener('click', function () {
        // Xóa trạng thái active cũ
        document.querySelectorAll('.menu-item').forEach(item => {
            item.classList.remove('menu-item--active');
        });

        // Đặt trạng thái active cho menu được nhấn
        this.parentElement.classList.add('menu-item--active');
    });
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

