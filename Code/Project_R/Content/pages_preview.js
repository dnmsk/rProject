/**
 * Created by sotty on 21.05.14.
 */
function checkFormHint(){
    var m = window.location.href.match(/form:([a-f0-9]{32})/i);
    if (null===m) return;

    var frm = $('form#frm'+m[1]+':visible');//console.log('form#frm'+m[1]+':visible',frm);
    if (!frm.length){
        frm = $('.blk#'+m[1]+' .blk_form_wrap'); //console.log('.blk#'+m[1]+' .blk_form_wrap',frm);
    }

    if (frm.length){
        $(document).ready(function(){
            frm.css('outline', '5px dashed #FFC119');
            var top = frm.offset().top, scrollY=0;
            var arrow = $('<div></div>');
            var section_inner = $('#sections_list .blk_section:first .blk_section_inner');
            section_inner.css({position:'relative'});
            arrow.appendTo( section_inner );// $('body'));
            var lft = frm.offset().left - section_inner.offset().left + frm.width()/2 - 35/*половина ширины стрелка*/;

            if (top > 120) {
                scrollY = frm.offset().top-120;
                arrow.addClass('big_arrow_top').css({top: scrollY-5, left: lft});
                arrow.animate({top: scrollY+15},2000);
            } else {
                scrollY = frm.offset().top-30;
                var t=frm.offset().top + frm.height() + 15 + 20;
                arrow.addClass('big_arrow_bottom').css({top: t, left: lft});
                arrow.animate({top: t-15},2000);
                t=null;
            }

            $('body').scrollTo(scrollY,function(){
                frm.animate({'outlineColor':'#FF7519'},200)
                    .animate({'outlineColor':'#FFC119'},200)
                    .animate({'outlineColor':'#FF7519'},200)
                    .animate({'outlineColor':'#FFC119'},200);
            });

            top=arrow=lft=section_inner=scrollY=null;
        });
    } else {
        alert('Такой формы нет на сайте :('+"\n"+'Возможно вы ее уже удалили');
    }

    m=null;
}
checkFormHint();