 
        function fillMenu(data) {
            if ($.isEmptyObject(data)) {
            $('#menu')
                .empty()
                .append($('<p>').text('No items available'));
             return;
            }
            $('#menu').empty();
        var table = $('<table class="table">');
        var theader = $('<tr>')
            .append($('<th>').text('Name'))
            .append($('<th>').text('Weight'))
            .append($('<th>').text('Price'))
            .append($('<th>'));
        table.append(theader);

        $.each(data, function () {
            var menuItem = this;
            var newRow = $('<tr>')
                .append($('<td>').text(menuItem.Name))
                .append($('<td>').text(menuItem.Weight + ' g'))
                .append($('<td>').text(numeral(menuItem.Price).format('$0,0.00')));

            var txt = "Details";
            if ($('#identity').length > 0) {
                txt = "Buy";                
            }
            var addLink = $('<a>').attr('name', menuItem.Id)
                .addClass('add-item')
                .attr('href', '/menu/createitem/' + this.Id)
                .text(txt);
            newRow.append($('<td>').append(addLink));

            table.append(newRow);
        });

        $('#menu').append(table);

        $('a.add-item').click(
            function () {
                var itemId = $(this).attr('name');
                addItem(itemId);
            });
    };

    $('button.category').click(function () {
        var category = $(this).val();
        $.getJSON('/menu/filtereditems', {categParam: category })
            .done(function (data) {
                fillMenu(data);
            });
    });

    function fillCart(order) {
        var div = $('<div>').append($('<a>').text('Your Shopping Cart:').attr('href', '/shoppingcart'));
        var ul = $('<ul>');

        $.each(order.OrderedItems, function () {
            var orderItem = this;
            var li = $('<li>').text(orderItem.Name + ' ' + orderItem.Price + ' $')
                .appendTo(ul);
        });
        div.append(ul);
        var bill = $('<p>').text('Total bill: ' + order.TotalBill + ' $').appendTo(div);
        $('#cart').append(div);
    };

    $(function () {
        if ($('#identity').length > 0) {
             $.getJSON('/menu/getorder')
                 .done(function (order) {
                   fillCart(order)
                 });
        }
    });


