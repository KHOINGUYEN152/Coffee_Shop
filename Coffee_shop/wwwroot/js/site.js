// AJAX add-to-cart handler and cart badge updater.
document.addEventListener('DOMContentLoaded', () => {
	const cartBadge = document.getElementById('cart-count-badge');
	const addButtons = document.querySelectorAll('.add-to-cart-btn');

	const updateCartBadge = (count) => {
		if (!cartBadge) {
			return;
		}

		cartBadge.textContent = `(${count})`;
	};

	addButtons.forEach((button) => {
		button.addEventListener('click', async () => {
			const productId = button.getAttribute('data-product-id');

			if (!productId) {
				return;
			}

			try {
				const response = await fetch(`/Cart/AddAjax?id=${encodeURIComponent(productId)}`, {
					method: 'POST',
					headers: {
						'X-Requested-With': 'XMLHttpRequest'
					}
				});

				if (!response.ok) {
					throw new Error('Unable to add product to cart.');
				}

				const data = await response.json();
				if (typeof data.cartCount === 'number') {
					updateCartBadge(data.cartCount);
				}

				if (!button.dataset.originalHtml) {
					button.dataset.originalHtml = button.innerHTML;
				}

				button.classList.add('btn-success');
				button.classList.remove('btn-primary');
				button.innerHTML = 'Đã thêm vào giỏ';

				setTimeout(() => {
					button.innerHTML = button.dataset.originalHtml || button.innerHTML;
					button.classList.remove('btn-success');
					button.classList.add('btn-primary');
				}, 1000);
			} catch (error) {
				console.error(error);
				alert('Không thể thêm sản phẩm vào giỏ hàng.');
			}
		});
	});
});
