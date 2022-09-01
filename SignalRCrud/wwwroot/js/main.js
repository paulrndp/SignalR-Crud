$(() => {
    var connection = new signalR.HubConnectionBuilder().withUrl("/signalServer").build();
    connection.start();
    connection.on("LoadProducts", function () {
        LoadData();
    })
    LoadData();
    function LoadData() {
        var tr = '';
        $.ajax({
            url: '/Products/GetProduct',
            method: 'GET',
            success: (result) => {
                $.each(result, (k, v) => {
                    tr += `<tr>
                             <td>${v.Item}</td>
                             <td>${v.Category}</td>
                             <td>${v.Price}</td>
                             <td>${v.Qty}</td>
                             <td class="text-center"><a href="../Products/Details?id=${v.Id}">Details | <a href="../Products/Edit?id=${v.Id}">Edit | <a href="../Products/Delete?id=${v.Id}">Delete</td>
                    </tr>`
                })
                $('#tableBody').html(tr);
            },
            error: (error) => {
                console.log(error);
            }
        });

    }
});