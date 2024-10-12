const sliderContainer = document.getElementById("list-slider");
const listSliders = sliderContainer.getElementsByClassName("slider-item");

let indexSlider = 0;

renderSlider(indexSlider);

$(document).ready(function () {
    $('#icon-right').click(() => {
        if(indexSlider < listSliders.length -1)
        {
            indexSlider += 1;
        }
        else{
            indexSlider = 0;
        }
        renderSlider()
    });

    $('#icon-left').click(() => {
        if(indexSlider > 0)
        {
            indexSlider -= 1;
        }
        else{
            indexSlider = listSliders.length - 1;
        }
        renderSlider();
    });
});


function renderSlider() {
    for(let i = 0 ; i < listSliders.length ; i++){
        listSliders[i].style.transform = `translateX(${(i - indexSlider) *100}%)`
    }
}


