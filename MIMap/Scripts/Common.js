

    function successTips() {
        toastr.options = {
            "positionClass": "toast-top-center",
            "hideDuration": "300",//消失的动画时间
            "timeOut": "2000",//展现时间
        }
        toastr.success('Success')
    }
    function errorTips(str) {
        toastr.options = {
            "positionClass": "toast-top-center",
            "hideDuration": "300",//消失的动画时间
            "timeOut": "2000",//展现时间
        }
        toastr.error(str);
    }
