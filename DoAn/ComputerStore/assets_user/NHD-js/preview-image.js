function previewSingleImage(event, previewId) {
    var file = event.target.files[0]; // Lấy file đầu tiên từ input

    if (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
            // Cập nhật src của ảnh preview tương ứng
            document.getElementById(previewId).src = e.target.result;
        };
        reader.readAsDataURL(file);
    } else {
        // Nếu không có file, xóa preview
        document.getElementById(previewId).src = '';
    }
}

