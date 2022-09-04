var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#outlaysDataTable').DataTable({
        "ajax": {
            "url": "/Outlay/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "cost", "width": "10%", "class": "text-center" },
            { "data": "description", "width": "15%", "class": "text-center" },
            { "data": "category.name", "width": "15%", "class": "text-center" },
            {
                "data": "createdDate",
                "render": function (data) {
                    return data.substring(0, 10);
                },
                "width": "10%",
                "class": "text-center"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<a href="/Outlay/Upsert?id=${data}" class="btn btn-primary mx-2 text-center"><i class="bi bi-pencil-square"></i> &nbsp; Edit</a>`;
                },
                "width": "15%",
                "class": "text-center"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `<a onclick="Delete('/Outlay/Delete?id=${data}')" class="btn btn-danger mx-2 text-center"><i class="bi bi-trash"></i> &nbsp; Delete</a>`;
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
                        location.reload(true);
                        toastr.success(data.message);
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}