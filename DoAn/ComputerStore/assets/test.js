// Làm bài tập tại đây
// function getRequestBodyFromValues(formValues) {

    

//     for (var value of Object.values(formValues)){
//         console.log(value);
//     }
    
// }

// function getRequestBodyFromValues(formValues) {
//     var newObj = {};
//     formValues.forEach(function(item) {
//     newObj[item.field] = item.value;
//     });
//     return newObj;
//     }

function getRequestBodyFromValues(formValues) {
    var obj=new Object();
    formValues.forEach(function(formValue){
    obj[formValue.field]=formValue.value
    });
    console.log(obj)
    return obj;
    }

getRequestBodyFromValues( { field: 'name', value: 'Sơn Đặng' });