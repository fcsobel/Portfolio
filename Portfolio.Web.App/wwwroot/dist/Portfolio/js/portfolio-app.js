var c3o = {};
c3o.Core = {};
c3o.Core.Data = {};

//https://developer.mozilla.org/en-US/Add-ons/Overlay_Extensions/XUL_School/JavaScript_Object_Management
//https://developer.mozilla.org/en-US/docs/Web/JavaScript/Inheritance_and_the_prototype_chain
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
/////////////////////////////////////////////////////////////
// Dividend Class
/////////////////////////////////////////////////////////////
c3o.Core.Data.Dividend = function (data, investment) {
	var self = this;

	if (data) {
		_.extend(this, data);

		// set pointer to parent investment
		this.investment = investment;
	};
};


// Dividend class methods
c3o.Core.Data.Dividend.prototype = {

	Total: function () {
		return this.dividend * this.investment.quantity;
	}

	//// Merge Additional product data into existing object
	//Merge: function (Dividend) {
	//	_.extend(this, investment);
	//	//this.summary = product.summary;
	//	//this.description = product.description;
	//	//this.content = product.content;
	//	//this.mediaObjects = product.mediaObjects;
	//}

	////get Ordered() { return this.orders && this.orders.length > 0; },
	////get Price() { return _.sum(this.ticketTypes, function (obj) { return obj.Price; }); },
};

c3o.Core.Data.InvestmentResponse = function (data) {
	var self = this;
	if (data) {
		this.accountId = data.accountId;
		this.investmentId = data.investmentId;
		this.name = data.name;
		this.quantity = data.quantity;
		//this.cost = data.cost;
		this.stockId = data.stockId;
		this.ticker = data.ticker;
		//this.price = data.price;
		//this.dividend = data.dividend;
		this.tags = data.tags;
	};
};


/////////////////////////////////////////////////////////////
// Investment Class
/////////////////////////////////////////////////////////////
c3o.Core.Data.Investment = function (data) {
	var self = this;

	if (data) {
		_.extend(this, data);

		// Transform Account Investments
		if (this.dividends != null && this.dividends != undefined) {
			this.dividends = $.map(this.dividends, function (item, i) {
				return new c3o.Core.Data.Dividend(item, self);
			});
		}
	};
};


// Investment class methods
c3o.Core.Data.Investment.prototype = {

	Price: function () {
			return this.price * this.quantity;
	},
	Income: function () {
		return (this.price * this.quantity * (this.dividend / 100));
	},


	//// Merge Additional product data into existing object
	//Merge: function (investment) {
	//	_.extend(this, investment);
	//	//this.summary = product.summary;
	//	//this.description = product.description;
	//	//this.content = product.content;
	//	//this.mediaObjects = product.mediaObjects;
	//}

	////get Ordered() { return this.orders && this.orders.length > 0; },
	////get Price() { return _.sum(this.ticketTypes, function (obj) { return obj.Price; }); },
};