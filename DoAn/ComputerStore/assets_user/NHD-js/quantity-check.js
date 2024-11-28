// Lấy giá trị max quantity từ Razor
const maxQuantity = 50;

// Lấy các phần tử HTML
const quantityInput = document.getElementById("quantity");
const increaseBtn = document.getElementById("increase");
const decreaseBtn = document.getElementById("decrease");
const errorMsg = document.getElementById("error");

// Hàm kiểm tra số lượng hợp lệ
function validateQuantity() {
    // Chỉ cho phép số nguyên
    quantityInput.value = quantityInput.value.replace(/\D/g, "");
    let currentValue = parseInt(quantityInput.value);

    if (isNaN(currentValue) || currentValue < 1) {
        quantityInput.value = 1; // Reset về giá trị tối thiểu
        errorMsg.style.display = "none";
    } else if (currentValue > maxQuantity) {
        quantityInput.value = maxQuantity; // Reset về giá trị tối đa
        errorMsg.style.display = "block";
    } else {
        errorMsg.style.display = "none";
    }
}

// Sự kiện tăng số lượng
increaseBtn.addEventListener("click", () => {
    let currentValue = parseInt(quantityInput.value) || 1;

    if (currentValue < maxQuantity) {
        quantityInput.value = currentValue + 1;
    }

    validateQuantity();
});

// Sự kiện giảm số lượng
decreaseBtn.addEventListener("click", () => {
    let currentValue = parseInt(quantityInput.value) || 1;

    if (currentValue > 1) {
        quantityInput.value = currentValue - 1;
    }

    validateQuantity();
});

// Sự kiện khi người dùng nhập trực tiếp số lượng
quantityInput.addEventListener("input", validateQuantity);
quantityInput.addEventListener("blur", validateQuantity); // Kiểm tra lại khi rời khỏi ô input