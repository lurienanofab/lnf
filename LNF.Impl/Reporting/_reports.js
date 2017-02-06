$(document).ready(function () {
    $.reporting.reports('client-database-report', {
        'render': function (r) {
            var getFullNameHtml = function (client) {
                var name = client.FName;
                if (client.MName != '')
                    name += ' ' + client.MName;
                name += ' ' + client.LName;
                return '<strong>' + name + ' (' + client.UserName + ' ' + client.ClientID + ')</strong>';
            };

            var clientGroup = function (client) {
                var result = r.group().append(
                        r.subgroup().html(getFullNameHtml(client)).css('color', '#FF0000')
                    ).append(
                        r.subgroup().html('<strong>Privileges: </strong>' + client.PrivList.join(', '))
                    ).append(
                        r.subgroup().html('<strong>Citizen: </strong>' + client.Citizen)
                    ).append(
                        r.subgroup().html('<strong>Gender: </strong>' + client.Gender)
                    ).append(
                        r.subgroup().html('<strong>Race: </strong>' + client.Race)
                    ).append(
                        r.subgroup().html('<strong>Ethnicity: </strong>' + client.Ethnicity)
                    ).append(
                        r.subgroup().html('<strong>Disability: </strong>' + client.Disability)
                    ).append(
                        r.subgroup().html('<strong>Technical Interest: </strong>' + client.TechnicalField)
                    ).append(
                        r.subgroup().html('<strong>Communities: </strong>' + client.CommunityList.join(', '))
                    );
                return result;
            };

            var getAccountsHtml = function (org) {
                var list = $('<ul/>');
                $.each(org.Accounts, function (index, value) {
                    list.append($('<li/>').html(value));
                });
                return list;
            }

            var orgGroup = function (org) {
                var result = r.group().append(
                        r.subgroup().html('<strong>Organization: </strong>' + org.OrgName).css({ 'font-size': '12pt' })
                    ).append(
                        r.subgroup().html('<strong>Department: </strong>' + org.Department + ', <strong>Role: </strong> ' + org.Role)
                    ).append(
                        r.subgroup().html('<strong>Phone: </strong>' + ((org.Phone == '') ? 'none on record' : org.Phone) + ', <strong>Email: </strong> ' + org.Email)
                    ).append(
                        r.subgroup().html('<strong>Managers: </strong>' + org.Managers.join('; '))
                    ).append(
                        r.subgroup().html('<strong>Billing Type: </strong>' + org.BillingType)
                    ).append(
                        r.subgroup().html('<strong>Accounts: </strong>').append(getAccountsHtml(org))
                    );
                return result;
            };

            var loadClientSelect = function (data) {
                var select = $('<select/>', { "class": "client-select" });
                select.append($('<option/>').val(0).html(''));
                $.each(data, function (index, value) {
                    select.append($('<option/>').attr('value', value[0]).html(value[2]));
                });
                r.output()
                    .find('.client-select-container')
                    .html('')
                    .append(select);
            };

            var loadClientDetail = function (clientId, callback) {
                r.serverRequest({
                    'data': { 'key': 'client-database-report', 'clientId': clientId, 'period': r.period(), 'resultType': 'Ajax' },
                    'success': function (json) {
                        r.detail().html('').append(clientGroup(json.Client));
                        $.each(json.Orgs, function (index, value) {
                            r.detail().append(orgGroup(value));
                        });
                        if (typeof callback == 'function')
                            callback();
                    },
                    'error': function (err) {
                    }
                });
            };

            //create the client select container
            $('<div/>')
                .css({ 'padding-bottom': '15px' })
                .html('<span class="nodata">Select a client from the drop down or click a table row:</span>')
                .append(
                    $('<div/>', { "class": "client-select-container" }).css({ 'padding-top': '5px' })
                ).appendTo(r.output());

            //create the client list
            var tbl = $('<table/>', { "class": "clients-table" })
                .append($('<thead><tr><th style="width: 50px;">ClientID</th><th style="width: 100px;">Username</th><th style="width: 300px;">Name</th><th>Privleges</th><th>Organizations</th></tr></thead>'))
                .append($('<tbody/>'))
                .appendTo(r.output())
                .dataTable({
                    'processing': true,
                    'ajax': { 'url': 'ajax?key=' + r.key() + '&clientId=0&period=' + r.period() + '&resultType=DataTablesAjax' },
                    'initComplete': function (oSettings, json) {
                        loadClientSelect(json.aaData);
                    }
                });

            //table row click event
            tbl.on('click', '.list-subitem', function (event) {
                event.stopPropagation();
                var text = $(this).html();
                tbl.fnFilter(text);
            }).on('mouseover', '.list-subitem', function (event) {
                $(this).css({ 'color': '#336699' });
            }).on('mouseout', '.list-subitem', function (event) {
                $(this).css({ 'color': '' });
            }).on('click', 'tr', function (event) {
                var data = tbl.fnGetData(this);
                var clientId = data[0];
                loadClientDetail(clientId);
            });

            //refresh click event
            r().on('click', '.refresh-button', function (event) {
                tbl.api().ajax.url('ajax?key=' + r.key() + '&clientId=0&period=' + r.period() + '&resultType=DataTablesAjax').load(function (oSettings) {
                    loadClientSelect(tbl.fnGetData());
                });
                //client select change event
            }).on('change', '.client-select', function (event) {
                var clientId = $(this).val();
                if (clientId == 0)
                    tbl.fnFilter('');
                else {
                    tbl.fnFilter($(this).find('option:selected').text());
                    loadClientDetail(clientId);
                }
            });

            r.detail().html('<span class="nodata">Select a client from the list above to view details.</span>');
        }
    }).reports('tool-utilization-report', {
        'render': function (r) {

            var table;

            var firstActivityCol = 5;

            var getCriteria = function () {
                return {
                    key: 'tool-utilization-report',
                    period: r.period(),
                    monthCount: r.criteria().find('.month-count-textbox').val(),
                    statsBasedOn: r.criteria().find('.stats-based-on-radios:checked').val(),
                    includeForgiven: r.criteria().find('.include-forgiven-checkbox').prop('checked'),
                    showPercent: r.criteria().find('.show-percent-checkbox').prop('checked')
                };
            };

            var calculateTotals = function (setup, aaData, iStart, iEnd, aiDisplay) {
                var totals = [];
                $.each(setup.activities, function (index, value) {
                    totals.push(0);
                });
                totals.push(0);
                for (index = iStart; index < iEnd; index++) {
                    var row = aaData[aiDisplay[index]];
                    for (i = 0; i < totals.length; i++) {
                        totals[i] += parseFloat(row[i + firstActivityCol]);
                    }
                };
                return totals;
            };

            var getAjaxSourceUrl = function () {
                var c = getCriteria();

                var result = r.baseUrl() + '/report/ajax/datatables/'
                    + '?key=tool-utilization-report'
                    + '&period=' + c.period
                    + '&monthCount=' + c.monthCount
                    + '&statsBasedOn=' + c.statsBasedOn
                    + '&includeForgiven=' + c.includeForgiven
                    + '&showPercent=' + c.showPercent;

                return result;
            };

            var getSelectedProcTech = function () {
                return $('.proctech-select', r.output()).find('option:selected').val()
            };

            var loadTable = function (setup) {
                r.loadTable(r.output(), {
                    'aoColumns': setup.aoColumns,
                    'sAjaxSource': getAjaxSourceUrl(),
                    'aaSorting': [[4, 'asc']],
                    'aLengthMenu': [
                        [25, 50, 100, -1],
                        [25, 50, 100, "All"]
                    ],
                    'fnFooterCallback': function (nRow, aaData, iStart, iEnd, aiDisplay) {
                        if (aaData.length == 0) return;

                        var tableColumns = setup.tableColumns;
                        var dataColumns = setup.dataColumns;
                        var activityCount = setup.activities.length;
                        var totals = new Array(activityCount + 1);
                        var activityStartCol = dataColumns - activityCount - 1;

                        //get a sum for each column
                        var totals = calculateTotals(setup, aaData, iStart, iEnd, aiDisplay);

                        var footer = $(nRow);
                        footer.html('').css({ 'font-weight': 'bold', 'background-color': '#ACF8A4' });
                        if (totals.length > 0) {
                            footer.append(
                                $('<td/>')
                                    .css({ 'text-align': 'right' })
                                    .prop('colspan', 2)
                                    .html('Totals:')
                            );
                            $.each(totals, function (index, value) {
                                $(nRow).append(
                                    $('<td/>')
                                        .css({ 'text-align': 'right' })
                                        .html(value.toFixed(3))
                                );
                            });
                        }
                    }
                }, function (tbl) {
                    tbl.on('click', 'tr', function (event) {
                        var criteria = getCriteria();
                        var key = 'tool-utilization-detail';
                        var data = tbl.fnGetData(this);
                        var resourceId = data[0];
                        var endPeriod = new Date(r.period());
                        endPeriod.setMonth(endPeriod.getMonth() + parseInt(criteria.monthCount));
                        $('<form method="POST"/>')
                            .prop('action', r.baseUrl() + '/report/' + key + '/' + resourceId + '/' + r.period() + '/' + r.formatDate(endPeriod))
                            .append($('<input type="hidden"/>').prop('name', 'statsBasedOn').prop('value', criteria.statsBasedOn))
                            .append($('<input type="hidden"/>').prop('name', 'returnTo').prop('value', 'tool-utilization-report'))
                            .appendTo('body')
                            .submit();
                    });

                    var procTechs = setup.procTechs;
                    var select = $('<select/>', { "class": "proctech-select" })
                        .append($('<option/>').val('').text('-- All --'));

                    $.each(procTechs, function (index, value) {
                        select.append($('<option/>').val(value).text(value));
                    });

                    r.output().prepend(
                        $('<div/>')
                            .css({ 'padding-bottom': '10px' })
                            .html('Filter by Process Tech: ')
                            .append(select)
                    );

                    var search = tbl.fnSettings().oPreviousSearch.sSearch;
                    var select = $('.proctech-select', r.output());
                    var option = select.find('option[value="' + search + '"]');
                    if (option.length > 0) {
                        select.find('option').prop('selected', false);
                        option.prop('selected', true);
                    }

                    table = tbl;
                });
            };

            var getData = function (criteria) {
                var data = $.extend({}, criteria, { 'resultType': 'Ajax' });
                r.serverRequest({
                    'data': data,
                    'success': function (json) {
                        loadTable(json.Data);
                    },
                    'error': function (err) {
                        alert('sorry, ajax error');
                    }
                });
            };

            getData(getCriteria());

            r().on('click', '.run-button', function (event) {
                table.api().ajax.url(getAjaxSourceUrl()).load();
            }).on('change', '.proctech-select', function (event) {
                table.api().ajax.url(getSelectedProcTech()).load();
            });

            r.detail().closest('section').hide();
        }
    }).reports('tool-utilization-detail', {
        'render': function (r) {

            r.loadTable(r.output(), null, function (tbl) {

            });

            r.detail().closest('section').hide();
        }
    });
});