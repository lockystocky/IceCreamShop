$('#backbtn').click(function () {
    window.location.href = '/menu';
});

$('.addbtn').click(function () {
    var curValue = $(this).prevAll('.quantity').text();
    if (parseInt(curValue) < 5) {
        curValue++;
        $(this).prevAll('.quantity').text(curValue);
        $(this).prevAll('.bind-quantity').val(curValue);
    }
});

$('.delbtn').click(function () {
    var curValue = $(this).prevAll('.quantity').text();
    if (parseInt(curValue) > 0) {
        curValue--;
        $(this).prevAll('.quantity').text(curValue);
        $(this).prevAll('.bind-quantity').val(curValue);
    }
});