/////////////////////////////////////////////////////////////
// account Class
/////////////////////////////////////////////////////////////
c3o.Core.Data.Account = function (data) {
	var self = this;

	if (data) {
		_.extend(this, data);

		// Transform Account Investments
		if (this.investments != null && this.investments != undefined) {
			this.investments = $.map(this.investments, function (item, i) {
				return new c3o.Core.Data.Investment(item);
			});
		}
	};
};

// Account class methods
c3o.Core.Data.Account.prototype = {

	Price: function () {
		if (!this.investments) {
			return 0;
		}
		return this.investments.reduce(function (total, item) {
			return total + item.Price();
		}, 0);
	},
	Income: function () {
		if (!this.investments) {
			return 0;
		}
		return this.investments.reduce(function (total, item) {
			return total + item.Income();
		}, 0);
	},

	
	// get flat list of dividends for all stocks
	Dividends: function () {
		merged = [];
		if (this.investments) {
			this.investments.forEach(function (item) {
				merged.push(item.dividends);
			});
		}
		return merged.flat();
	},

	// get flat list of dividends for all stocks
	Months: function () {
		
		var months = [
			{ total: 0, month: 1, },
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

		this.Dividends().forEach(function (item) {
			var d = new Date(item.date);			
			var n = d.getMonth();
			months[n].total = months[n].total + item.Total();
		});

		return months;
	},

	// Merge Additional product data into existing object
	Merge: function (account) {
		_.extend(this, account);
		//this.summary = product.summary;
		//this.description = product.description;
		//this.content = product.content;
		//this.mediaObjects = product.mediaObjects;
	}

	//get Ordered() { return this.orders && this.orders.length > 0; },
	//get Price() { return _.sum(this.ticketTypes, function (obj) { return obj.Price; }); },
};