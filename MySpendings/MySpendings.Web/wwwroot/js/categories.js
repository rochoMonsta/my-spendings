var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#categoriesDataTable').DataTable({
        "ajax": {
            "url": "/Category/GetAll"
        },
        "columns": [
            { "data": "name", "width": "20%" },
            { "data": "description", "width": "20%", "class": "text-center" },
            { "data": "priority", "width": "20%", "class": "text-center" },
            {
                "data": "id",
                "render": function (data) {
                    return `<a href="/Category/Upsert?id=${data}" class="btn btn-primary mx-2 text-center"><i class="bi bi-pencil-square"></i> &nbsp; Edit</a>`;
                },
                "width": "20%",
                "class": "text-center"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<a onclick="Delete('/Category/Delete?id=${data}')" class="btn btn-danger mx-2 text-center"><i class="bi bi-trash"></i> &nbsp; Delete</a>`;
                },
                "width": "20%",
                "class": "text-center"
            },
        ]
    });
}

function Delete(url) {
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
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}