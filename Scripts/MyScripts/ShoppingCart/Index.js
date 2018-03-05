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

function setNewPriceAndBill(quantEl, newQuant, data) {
    quantEl.text(newQuant);
    var priceEl = quantEl.closest('td').next('td');
    priceEl.text(numeral(data[0]).format('$0,0.00'));
    $('#totalBill').text(numeral(data[1]).format('$0,0.00'));
}

$('.decreasebtn').click(function () {
    var itemId = $(this).val();
    var quantEl = $(this).siblings('.quantity');
    var priceEl = $(this).closest('td').next('td');
    var curQuant = (parseInt(quantEl.text()));
    if (curQuant < 1) return;
    $.post('/shoppingcart/changeitemquantity', {
        id: itemId,
        newQuantity: --curQuant
    },
        function (data) {
            setNewPriceAndBill(quantEl, curQuant, data);
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
        function (data) {
            setNewPriceAndBill(quantEl, curQuant, data);
        });
});