$('#tomenu').click(function () {
    window.location.href = '/menu/index';
});

$('.btndel').click(function (event) {
    var itemId = $(this).val();
    var item = $(this).closest('li');
    $.post('/shoppingcart/removeitem',
        {
            id: itemId
        },
        function () {
            item.hide();
            window.location.reload(true);
        });
});

$('.decreasebtn').click(function () {
    var itemId = $(this).val();
    var quantEl = $(this).siblings('.quantity');
    var curQuant = (parseInt(quantEl.text()));
    if (curQuant < 1) return;
    $.post('/shoppingcart/changeitemquantity', {
        id: itemId,
        newQuantity: --curQuant
    },
        function () {
            quantEl.text(curQuant);
        });
});

$('.increasebtn').click(function () {
    var itemId = $(this).val();
    var quantEl = $(this).siblings('.quantity');
    var curQuant = (parseInt(quantEl.text()));

    $.post('/shoppingcart/changeitemquantity', {
        id: itemId,
        newQuantity: ++curQuant
    },
        function () {
            quantEl.text(curQuant);
        });
});