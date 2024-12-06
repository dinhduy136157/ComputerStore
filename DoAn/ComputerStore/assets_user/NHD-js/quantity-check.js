// Lấy giá trị max quantity từ Razor
const maxQuantity = 5;

const cartItems = document.querySelectorAll(".m-cart-item");

cartItems.forEach((item) => {
    const quantityInput = item.querySelector(".quantity");
    const increaseBtn = item.querySelector(".increase");
    const decreaseBtn = item.querySelector(".decrease");
    const errorMsg = item.querySelector(".m-error");

    function validateQuantity() {
        quantityInput.value = quantityInput.value.replace(/\D/g, "");
        let currentValue = parseInt(quantityInput.value);

        if (isNaN(currentValue) || currentValue < 1) {
            quantityInput.value = 1;
            errorMsg.style.display = "none";
        } else if (currentValue > maxQuantity) {
            quantityInput.value = maxQuantity;
            errorMsg.style.display = "block";
        } else {
            errorMsg.style.display = "none";
        }
    }

    increaseBtn.addEventListener("click", () => {
        let currentValue = parseInt(quantityInput.value) || 1;

        if (currentValue < maxQuantity) {
            quantityInput.value = currentValue + 1;
        }

        validateQuantity();
    });

    decreaseBtn.addEventListener("click", () => {
        let currentValue = parseInt(quantityInput.value) || 1;

        if (currentValue > 1) {
            quantityInput.value = currentValue - 1;
        }

        validateQuantity();
    });

    quantityInput.addEventListener("input", validateQuantity);
    quantityInput.addEventListener("blur", validateQuantity);
});