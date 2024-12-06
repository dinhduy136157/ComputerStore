document.getElementById("saveAddressBtn").addEventListener("click", function () {
    var txtUserFullName = document.getElementById("txtUserFullName").value;
    var txtUserPhone = document.getElementById("txtUserPhone").value;
    var cbbCity = document.getElementById("cbbCity").value;
    var cbbDistrict = document.getElementById("cbbDistrict").value;
    var cbbCommune = document.getElementById("cbbCommune").value;
    var txtUserSpecificAddress = document.getElementById("txtUserSpecificAddress").value;

    // Cập nhật view 
    document.getElementById("currentUserFullName").innerText = txtUserFullName;
    document.getElementById("currentUserPhone").innerText = txtUserPhone;
    document.getElementById("currentUserCity").innerText = cbbCity;
    document.getElementById("currentUserDistrict").innerText = cbbDistrict;
    document.getElementById("currentUserCommune").innerText = cbbCommune;
    document.getElementById("currentUserSpecificAddress").innerText = txtUserSpecificAddress;

    // Đóng modal
    $('#addressModal').modal('hide');
});

// #region Gửi về server bằng ajax
//document.getElementById("saveAddressButton").addEventListener("click", function () {
//    var addressText = document.getElementById("addressText").value;
//    var citySelect = document.getElementById("citySelect").value;

//    // Gửi dữ liệu qua AJAX
//    $.ajax({
//        url: '/YourController/SaveAddress', // Thay bằng URL của bạn
//        type: 'POST',
//        data: {
//            Address: addressText,
//            City: citySelect
//        },
//        success: function (response) {
//            // Cập nhật view nếu thành công
//            document.getElementById("currentAddress").innerText = response.Address;
//            document.getElementById("currentCity").innerText = response.City;

//            // Đóng modal
//            $('#addressModal').modal('hide');
//        },
//        error: function () {
//            alert("Lưu địa chỉ thất bại!");
//        }
//    });
//});


// #endregion