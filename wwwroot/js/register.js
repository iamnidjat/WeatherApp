$(function f() {
    let $text = $('#Password')
    let $text2 = $('#ConfirmPassword')

    $(document).ready(function () {
        $('#show_password').hover(function show() {
            //Change the attribute to text
            $text.attr('type', 'text');
            $('.icon').removeClass('fa fa-eye-slash').addClass('fa fa-eye');
        },
            function () {
                //Change the attribute back to password
                $text.attr('type', 'password');
                $('.icon').removeClass('fa fa-eye').addClass('fa fa-eye-slash');
            });

        $('#show_password2').hover(function show() {
            //Change the attribute to text
            $text2.attr('type', 'text');
            $('.icon').removeClass('fa fa-eye-slash').addClass('fa fa-eye');
        },
            function () {
                //Change the attribute back to password
                $text2.attr('type', 'password');
                $('.icon').removeClass('fa fa-eye').addClass('fa fa-eye-slash');
            });
    });
})