$('.register-form .tab a').on('click', function (e) {

    e.preventDefault();

    $(this).parent().addClass('active');
    $(this).parent().siblings().removeClass('active');

    target = $(this).attr('href');

    $('.register-form .tab-content > div').not(target).hide();

    $(target).fadeIn(600);

});