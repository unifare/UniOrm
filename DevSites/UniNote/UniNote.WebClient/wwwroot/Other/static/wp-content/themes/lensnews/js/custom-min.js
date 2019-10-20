// 边栏随窗口移动
$(function(){0<$(".move").length&&$(".move").scrollChaser({wrapper:".content",offsetTop:10})}),
//回顶部
$(function(){
//当滚动条的位置处于距顶部100像素以下时，跳转链接出现，否则消失  
$(function(){$(window).scroll(function(){300<$(window).scrollTop()?$("#back-to-top").fadeIn(500):$("#back-to-top").fadeOut(500)}),
//当点击跳转链接后，回到页面顶部位置  
$("#back-to-top").click(function(){return $("body,html").animate({scrollTop:0},800),!1}),
//跳转到评论框
$("#back-to-comment").click(function(){$("html,body").animate({scrollTop:$("#respond").offset().top},800)})})}),
//面包屑分类添加子菜单Class
jQuery(document).ready(function(t){t("li.cat-item:has(ul.children)").addClass("cat-item-has-children")});
//文章小工具幻灯片
var swiper=new Swiper(".swiper-slidepost",{pagination:".swiper-slidepost-pagination",nextButton:".swiper-slidepost-button-next",prevButton:".swiper-slidepost-button-prev",paginationClickable:!0,centeredSlides:!0,autoplay:5e3,autoplayDisableOnInteraction:!1,lazyLoading:!0,keyboardControl:!0,loop:!0}),swiper=new Swiper(".swiper-bulletin",{nextButton:".swiper-bulletin-next",prevButton:".swiper-bulletin-prev",direction:"vertical",autoplay:5e3,slidesPerView:1,paginationClickable:!0,autoplayDisableOnInteraction:!1,spaceBetween:30,loop:!0});
//公告滚动
// 简码中的开关盒
jQuery(document).ready(function(){
// Toggle Box
jQuery("ul.gdl-toggle li,ul.gdl-toggle-box li").each(function(){jQuery(this).children(".toggle-content,.toggle-box-content").not(".active").css("display","none"),$("ul.gdl-toggle li:first .toggle-head .icon-plus,ul.gdl-toggle-box li:first .toggle-box-head .icon-plus").addClass("active"),$("ul.gdl-toggle li:first .toggle-content,ul.gdl-toggle-box li:first .toggle-box-content").show(),jQuery(this).children(".toggle-head,.toggle-box-head").bind("click",function(){jQuery(this).children().addClass(function(){return jQuery(this).hasClass("active")?(jQuery(this).removeClass("active"),""):"active"}),jQuery(this).siblings(".toggle-content,.toggle-box-content").slideToggle()})})}),
// TABS
$(function(){$("#tabs li:first,#tab_content > div:first").addClass("current"),$("#tabs li a").click(function(t){$("#tabs li, #tab_content .current").removeClass("current").removeClass("fadeInLeft"),$(this).parent().addClass("current");var e=$(this).attr("href");$(e).addClass("current fadeInLeft"),t.preventDefault()})}),
//移动设备侧边按钮
$(".footer-popup .show").click(function(){$(".footer-popup").toggleClass("mobile-btn")}),
/*点击搜索按钮光标在输入框上，并设置一些延迟*/
$(document).ready(function(){$("ul.menu li.menu-item-search a,.side_btn.search").click(function(){setTimeout(function(){$(".search_form #s").focus()},500)})}),
/*购物车*/
$("a.cart_btn").click(function(){$(".ajax_cart").toggleClass("active"),$(".bg.cart").toggleClass("active")}),$(".bg.cart").click(function(){$(".ajax_cart,.bg.cart").removeClass("active")});
