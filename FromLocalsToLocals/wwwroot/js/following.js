function followBtnClick(obj, id) {

    var fButtons = $(`.followBtn[data-id='${id}']`);;
    $.ajax({
        type: "POST",
        url: '/Follower/' + obj.innerText + '/' + id,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (m) {
            if (m.success) {
                if (obj.innerText == "Follow") {
                    for (var i = 0; i < fButtons.length; i++) {
                        fButtons[i].innerText = "Unfollow";
                        fButtons[i].className = "btn btn-outline-dark w-100 followBtn"
                    }
                } else if (obj.innerText == "Unfollow") {
                    for (var i = 0; i < fButtons.length; i++) {
                        fButtons[i].innerText = "Follow";
                        fButtons[i].className = "btn btn-dark w-100 followBtn"
                    }
                }
            } else {
                alert("Error occured when trying to " + obj.innerText + " vendor");
            }
        },
        error: function (e) {
            alert("Error occured when trying to " + obj.innerText + " vendor");
        }
    });
}