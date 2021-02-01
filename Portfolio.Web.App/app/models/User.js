/////////////////////////////////////////////////////////////
// User Class
/////////////////////////////////////////////////////////////
c3o.Core.Data.User = function (data) {
	var self = this;

	if (data) {
		_.extend(this, data);

		if (!this.accounts) this.accounts = [];

		// Transform User Accounts
		if (this.accounts != null && this.accounts != undefined) {
			this.accounts = $.map(this.accounts, function (item, i) {
				return new c3o.Core.Data.Account(item);
			});
		}
	};
};

// User class methods
c3o.Core.Data.User.prototype = {

	Price: function () {
		if (!this.accounts) {
			return 0;
		}
		return this.accounts.reduce(function (total, item) {
			return total + item.Price();
		}, 0);
	},
	Income: function () {
		if (!this.accounts) {
			return 0;
		}
		return this.accounts.reduce(function (total, item) {
			return total + item.Income();
		}, 0);
	},

	
	Months: function () {

		var months = [
			{ total: 0, month: 1 },
			{ total: 0, month: 2 },
			{ total: 0, month: 3 },
			{ total: 0, month: 4 },
			{ total: 0, month: 5 },
			{ total: 0, month: 6 },
			{ total: 0, month: 7 },
			{ total: 0, month: 8 },
			{ total: 0, month: 9 },
			{ total: 0, month: 10 },
			{ total: 0, month: 11 },
			{ total: 0, month: 12 },
		];

		if (this.accounts) {
			this.accounts.forEach(function (item) {
				item.Months().forEach(function (month) {
					var n = month.month - 1;
					months[n].total = months[n].total + month.total;
				});

			});
		}

		return months;
	},



	// Merge Additional product data into existing object
	Merge: function (user) {
		_.extend(this, user);
		//this.summary = product.summary;
		//this.description = product.description;
		//this.content = product.content;
		//this.mediaObjects = product.mediaObjects;
	}

	//get Ordered() { return this.orders && this.orders.length > 0; },
	//get Price() { return _.sum(this.ticketTypes, function (obj) { return obj.Price; }); },
};