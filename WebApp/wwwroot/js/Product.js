var table;
$(document).ready(function () {
    table = $('#myTable').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAllProducts"    
        },
        "columns": [
            { "data": "productName" },
            { "data": "description" },
            { "data": "price" },
            { "data": "category.categoryName" },
            {
                "data": "productId",
                "render": function (data) {
                    return `<a href="/Admin/Product/CreateUpdate?id=${data}"><i class="bi bi-pencil-square"></i></a>
                        <a onClick=RemoveProduct("/Admin/Product/Delete/${data}")><i class="bi bi-trash"></i></a>`
                }
            }
        ]

    });
});

function RemoveProduct(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function(data){
                    if (data.success) {
                        table.ajax.reload();
                        toastr.success(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}