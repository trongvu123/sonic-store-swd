$(document).ready(function () {
    var homeconfig = {
        pageSize: 10,
        pageIndex: 1
    };
    var pageSize = 10;
    var pageIndex = 1;
    var loadedProducts = 0;
    var selectedBrand = null;
    var selectedPrice = null;
    var selectedCategory = null;
    var selectedRam = null;
    var selectedStorage = null;
    var selectedSort = null;
    var selectedDelete = null;
    function checkIfEmpty() {
        if ($('.list-products').children('.list-products__item').length === 0) {
            $('.empty-title').fadeIn();
        } else {
            $('.empty-title').fadeOut();
        }
    }
    function loadData(changePageSize) {
        let url = "loaddata?skip=" + loadedProducts;
        if (selectedBrand) {
            url += "&brand=" + selectedBrand;
        }
        if (selectedPrice) {
            url += "&price=" + selectedPrice;
        }
        if (selectedCategory) {
            url += "&category=" + selectedCategory;
        }
        if (selectedRam) {
            url += "&ram=" + selectedRam;
        }
        if (selectedStorage) {
            url += "&storage=" + selectedStorage;
        }
        if (selectedSort !== null) {
            url += "&sort=" + selectedSort;
        }
        if (selectedDelete !== null) {
            url += "&delete=" + selectedDelete;
        }
        $.ajax({
            url: url,
            data: {
                page: pageIndex,
                pageSize: pageSize
            },
            type: "GET",
            dataType: "json",
            success: function (response) {
                checkIfEmpty();
                if (response.status) {
                    var data = response.data;
                    var html = '';
                    var template = $('#data-template').html();
                    $.each(data, function (i, item) {
                        html += Mustache.render(template, {
                            name: item.name,
                            image: item.image,
                            originalPrices: item.originalPrices,
                            salePrices: item.salePrices,
                            url: item.url
                        });
                    });
                    $('.list-products').html(html);
                    Paging(response.total, function () {
                        loadData();
                    }, changePageSize);
                    loadedProducts += data.length;

                    $(".ex_pricesale").each(function () {
                        var price = $(this).attr("data-price");
                        var price1 = $(this).attr("data-price1");
                        if (price1 > price) {
                            $(this).removeClass('d-none');
                            $(this).html(100 - (Math.round((price / price1) * 100)) + '%');
                        }
                    });
                    var formatter = new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' });
                    var priceTags = document.querySelectorAll('.js-format-price');
                    priceTags.forEach(tag => {
                        var price = tag.getAttribute('data-price');
                        var formattedPrice = formatter.format(price);
                        tag.textContent = formattedPrice;
                    });

                    var saleTags = document.querySelectorAll('.ex_pricesale');
                    saleTags.forEach(tag => {
                        var price = parseFloat(tag.getAttribute('data-price'));
                        var originalPrice = parseFloat(tag.getAttribute('data-price1'));
                        var discountPercent = ((originalPrice - price) / originalPrice) * 100;
                        var roundedDiscountPercent = Math.round(discountPercent);
                        tag.textContent = roundedDiscountPercent + '%';
                    });
                    
                }
            },
            error: function (error) {
                console.error(error);
            }
        });
    }
    function Paging(total, callback, changePageSize) {
        var totalPage = Math.ceil(total / pageSize);

        if ($('#pagination a').length === 0 || changePageSize === true) {
            $('#pagination').empty();
            $('#pagination').removeData("twbs-pagination");
            $('#pagination').unbind("page");
        }
        $('#pagination').twbsPagination({
            totalPages: totalPage,
            first: "Đầu",
            next: "Tiếp",
            last: "Cuối",
            prev: "Trước",
            visiblePages: 5,
            onPageClick: function (event, page) {
                pageIndex = page;
                setTimeout(callback, 200);
            }
        });
    }
    $('.delete-option').on('click', function () {
        selectedDelete = $(this).data('delete');
        selectedBrand = null;
        selectedPrice = null;
        selectedCategory = null;
        selectedRam = null;
        selectedStorage = null;
        selectedSort = null;
        loadedProducts = 0;
        $('.delete-option').fadeOut();
        $('.facet strong').text('');
        $('#sort-list a').removeClass('active');
        loadData(true);
    });
    loadData();
    $('#brand-list').on('click', '.brand-link', function (event) {
        event.preventDefault();
        selectedBrand = $(this).data('brand-id');
        loadedProducts = 0;
        selectedDelete = null;
        $('.delete-option').fadeIn();
        $('.list-products').empty();
        loadData(true);
    });

    $('#price-list').on('click', '.price-link', function (event) {
        event.preventDefault();
        selectedPrice = $(this).data('price');
        loadedProducts = 0;
        selectedDelete = null;
        $('.delete-option').fadeIn();
        $('.list-products').empty();
        loadData(true);
    });

    $('#category-list').on('click', '.category-link', function (event) {
        event.preventDefault();
        selectedCategory = $(this).data('category-id');
        loadedProducts = 0;
        selectedDelete = null;
        $('.delete-option').fadeIn();
        $('.list-products').empty();
        loadData(true);
    });

    $('#ram-list').on('click', '.ram-link', function (event) {
        event.preventDefault();
        selectedRam = $(this).data('ram');
        loadedProducts = 0;
        selectedDelete = null;
        $('.delete-option').fadeIn();
        $('.list-products').empty();
        loadData(true);
    });
    $('#storage-list').on('click', '.storage-link', function (event) {
        event.preventDefault();
        selectedStorage = $(this).data('storage');
        loadedProducts = 0;
        selectedDelete = null;
        $('.delete-option').fadeIn();
        $('.list-products').empty();
        loadData(true);
    });
    $('#sort-list').on('click', '.sort-link', function (event) {
        event.preventDefault();
        selectedSort = $(this).data('sort');
        loadedProducts = 0;
        selectedDelete = null;
        $('#sort-list a').removeClass('active');
        $(this).addClass('active');
        $('.delete-option').fadeIn();
        $('.list-products').empty();
        loadData(true);
    });
    $('.facet').on('click', 'ul li a', function () {
        var storageContent = $(this).text().trim().replace($(this).find('i').text(), '');
        $(this).closest('.facet').find('strong').text(storageContent);
    });
    $('.btn-main').click(function () {
        loadData();
        selectedDelete = null;
    });

});

