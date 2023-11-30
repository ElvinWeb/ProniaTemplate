let deleteBtns = document.querySelectorAll(".delete-btn");

deleteBtns.forEach(delBtn => delBtn.addEventListener("click", (e) => {
    e.preventDefault();
    let url = delBtn.getAttribute("href")

    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {

            fetch(url).then(res => {
                if (res.ok == true) {
                    window.location.reload(true);

                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Oops...",
                        text: "Something went wrong!",
                    });
                }

            })
        }
    });
}))
