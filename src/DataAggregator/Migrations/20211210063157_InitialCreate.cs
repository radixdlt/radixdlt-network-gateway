/* Copyright 2021 Radix Publishing Ltd incorporated in Jersey (Channel Islands).
 *
 * Licensed under the Radix License, Version 1.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at:
 *
 * radixfoundation.org/licenses/LICENSE-v1
 *
 * The Licensor hereby grants permission for the Canonical version of the Work to be
 * published, distributed and used under or by reference to the Licensor’s trademark
 * Radix ® and use of any unregistered trade names, logos or get-up.
 *
 * The Licensor provides the Work (and each Contributor provides its Contributions) on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied,
 * including, without limitation, any warranties or conditions of TITLE, NON-INFRINGEMENT,
 * MERCHANTABILITY, or FITNESS FOR A PARTICULAR PURPOSE.
 *
 * Whilst the Work is capable of being deployed, used and adopted (instantiated) to create
 * a distributed ledger it is your responsibility to test and validate the code, together
 * with all logic and performance of that code under all foreseeable scenarios.
 *
 * The Licensor does not make or purport to make and hereby excludes liability for all
 * and any representation, warranty or undertaking in any form whatsoever, whether express
 * or implied, to any entity or person, including any representation, warranty or
 * undertaking, as to the functionality security use, value or other characteristics of
 * any distributed ledger nor in respect the functioning or value of any tokens which may
 * be created stored or transferred using the Work. The Licensor does not warrant that the
 * Work or any use of the Work complies with any law or regulation in any territory where
 * it may be implemented or used or that it will be appropriate for any specific purpose.
 *
 * Neither the licensor nor any current or former employees, officers, directors, partners,
 * trustees, representatives, agents, advisors, contractors, or volunteers of the Licensor
 * shall be liable for any direct or indirect, special, incidental, consequential or other
 * losses of any kind, in tort, contract or otherwise (including but not limited to loss
 * of revenue, income or profits, or loss of use or data, or loss of reputation, or loss
 * of any economic or other opportunity of whatsoever nature or howsoever arising), arising
 * out of or in connection with (without limitation of any use, misuse, of any ledger system
 * or use made or its functionality or any performance or operation of any code or protocol
 * caused by bugs or programming or logic errors or otherwise);
 *
 * A. any offer, purchase, holding, use, sale, exchange or transmission of any
 * cryptographic keys, tokens or assets created, exchanged, stored or arising from any
 * interaction with the Work;
 *
 * B. any failure in a transmission or loss of any token or assets keys or other digital
 * artefacts due to errors in transmission;
 *
 * C. bugs, hacks, logic errors or faults in the Work or any communication;
 *
 * D. system software or apparatus including but not limited to losses caused by errors
 * in holding or transmitting tokens by any third-party;
 *
 * E. breaches or failure of security including hacker attacks, loss or disclosure of
 * password, loss of private key, unauthorised use or misuse of such passwords or keys;
 *
 * F. any losses including loss of anticipated savings or other benefits resulting from
 * use of the Work or any changes to the Work (however implemented).
 *
 * You are solely responsible for; testing, validating and evaluation of all operation
 * logic, functionality, security and appropriateness of using the Work for any commercial
 * or non-commercial purpose and for any reproduction or redistribution by You of the
 * Work. You assume all risks associated with Your use of the Work and the exercise of
 * permissions under this License.
 */

﻿using System;
using System.Numerics;
using Common.Database.Models.Mempool;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAggregator.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mempool_transactions",
                columns: table => new
                {
                    transaction_id = table.Column<byte[]>(type: "bytea", nullable: false),
                    payload = table.Column<byte[]>(type: "bytea", nullable: false),
                    submitted_by_this_gateway = table.Column<bool>(type: "boolean", nullable: false),
                    first_submitted_to_gateway_timestamp = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    last_submitted_to_gateway_timestamp = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    last_submitted_to_node_timestamp = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    last_submitted_to_node_name = table.Column<string>(type: "text", nullable: true),
                    first_seen_in_mempool_timestamp = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    last_seen_in_mempool_timestamp = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    commit_timestamp = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    transaction_contents = table.Column<GatewayTransactionContents>(type: "jsonb", nullable: false),
                    submission_status = table.Column<string>(type: "text", nullable: false),
                    submission_failure_reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mempool_transactions", x => x.transaction_id);
                });

            migrationBuilder.CreateTable(
                name: "network_configuration",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    network_name = table.Column<string>(type: "text", nullable: false),
                    account_hrp = table.Column<string>(type: "text", nullable: false),
                    resource_hrp_suffix = table.Column<string>(type: "text", nullable: false),
                    validator_hrp = table.Column<string>(type: "text", nullable: false),
                    node_hrp = table.Column<string>(type: "text", nullable: false),
                    xrd_address = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_network_configuration", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "nodes",
                columns: table => new
                {
                    name = table.Column<string>(type: "text", nullable: false),
                    address = table.Column<string>(type: "text", nullable: false),
                    trust_weighting = table.Column<decimal>(type: "numeric", nullable: false),
                    enabled_for_indexing = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nodes", x => x.name);
                });

            migrationBuilder.CreateTable(
                name: "raw_transactions",
                columns: table => new
                {
                    transaction_id = table.Column<byte[]>(type: "bytea", nullable: false),
                    payload = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_raw_transactions", x => x.transaction_id);
                });

            migrationBuilder.CreateTable(
                name: "ledger_transactions",
                columns: table => new
                {
                    state_version = table.Column<long>(type: "bigint", nullable: false),
                    transaction_id = table.Column<byte[]>(type: "bytea", nullable: false),
                    transaction_accumulator = table.Column<byte[]>(type: "bytea", nullable: false),
                    message = table.Column<byte[]>(type: "bytea", nullable: true),
                    fee_paid = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    epoch = table.Column<long>(type: "bigint", nullable: false),
                    index_in_epoch = table.Column<long>(type: "bigint", nullable: false),
                    round_in_epoch = table.Column<long>(type: "bigint", nullable: false),
                    is_only_round_change = table.Column<bool>(type: "boolean", nullable: false),
                    is_start_of_epoch = table.Column<bool>(type: "boolean", nullable: false),
                    is_start_of_round = table.Column<bool>(type: "boolean", nullable: false),
                    round_timestamp = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    created_timestamp = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    timestamp = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ledger_transactions", x => x.state_version);
                    table.UniqueConstraint("AK_ledger_transactions_transaction_accumulator", x => x.transaction_accumulator);
                    table.UniqueConstraint("AK_ledger_transactions_transaction_id", x => x.transaction_id);
                    table.ForeignKey(
                        name: "FK_ledger_transactions_raw_transactions_transaction_id",
                        column: x => x.transaction_id,
                        principalTable: "raw_transactions",
                        principalColumn: "transaction_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "accounts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    address = table.Column<string>(type: "text", nullable: false),
                    public_key = table.Column<byte[]>(type: "bytea", nullable: false),
                    from_state_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_accounts", x => x.id);
                    table.ForeignKey(
                        name: "FK_account_from_transaction",
                        column: x => x.from_state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resources",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    engine_address = table.Column<byte[]>(type: "bytea", nullable: false),
                    rri = table.Column<string>(type: "text", nullable: false),
                    from_state_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resources", x => x.id);
                    table.ForeignKey(
                        name: "FK_resource_from_transaction",
                        column: x => x.from_state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "validators",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    address = table.Column<string>(type: "text", nullable: false),
                    public_key = table.Column<byte[]>(type: "bytea", nullable: false),
                    from_state_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validators", x => x.id);
                    table.ForeignKey(
                        name: "FK_validator_from_transaction",
                        column: x => x.from_state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_transactions",
                columns: table => new
                {
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    state_version = table.Column<long>(type: "bigint", nullable: false),
                    is_fee_payer = table.Column<bool>(type: "boolean", nullable: false),
                    is_signer = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_transactions", x => new { x.account_id, x.state_version });
                    table.ForeignKey(
                        name: "FK_account_transactions_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_transactions_ledger_transactions_state_version",
                        column: x => x.state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_resource_balance_history",
                columns: table => new
                {
                    from_state_version = table.Column<long>(type: "bigint", nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    resource_id = table.Column<long>(type: "bigint", nullable: false),
                    balance = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    to_state_version = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_resource_balance_history", x => new { x.account_id, x.resource_id, x.from_state_version });
                    table.ForeignKey(
                        name: "FK_account_resource_balance_history_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_resource_balance_history_ledger_transactions_from_s~",
                        column: x => x.from_state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_resource_balance_history_ledger_transactions_to_sta~",
                        column: x => x.to_state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version");
                    table.ForeignKey(
                        name: "FK_account_resource_balance_history_resources_resource_id",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resource_supply_history",
                columns: table => new
                {
                    from_state_version = table.Column<long>(type: "bigint", nullable: false),
                    resource_id = table.Column<long>(type: "bigint", nullable: false),
                    total_supply = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    total_minted = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    total_burnt = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    to_state_version = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_supply_history", x => new { x.resource_id, x.from_state_version });
                    table.ForeignKey(
                        name: "FK_resource_supply_history_from_transaction",
                        column: x => x.from_state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resource_supply_history_resources_resource_id",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resource_supply_history_to_transaction",
                        column: x => x.to_state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "account_validator_stake_history",
                columns: table => new
                {
                    from_state_version = table.Column<long>(type: "bigint", nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    validator_id = table.Column<long>(type: "bigint", nullable: false),
                    total_stake_units = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    total_prepared_xrd_stake = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    total_prepared_unstake_units = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    total_exiting_xrd_stake = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    to_state_version = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_validator_stake_history", x => new { x.account_id, x.validator_id, x.from_state_version });
                    table.ForeignKey(
                        name: "FK_account_validator_stake_history_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_validator_stake_history_from_transaction",
                        column: x => x.from_state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_validator_stake_history_to_transaction",
                        column: x => x.to_state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_account_validator_stake_history_validators_validator_id",
                        column: x => x.validator_id,
                        principalTable: "validators",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "operation_groups",
                columns: table => new
                {
                    state_version = table.Column<long>(type: "bigint", nullable: false),
                    operation_group_index = table.Column<int>(type: "integer", nullable: false),
                    inferred_action_type = table.Column<string>(type: "text", nullable: true),
                    inferred_action_from_account_id = table.Column<long>(type: "bigint", nullable: true),
                    inferred_action_to_account_id = table.Column<long>(type: "bigint", nullable: true),
                    inferred_action_validator_id = table.Column<long>(type: "bigint", nullable: true),
                    inferred_action_amount = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: true),
                    inferred_action_resource_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_operation_groups", x => new { x.state_version, x.operation_group_index });
                    table.ForeignKey(
                        name: "FK_operation_groups_accounts_inferred_action_from_account_id",
                        column: x => x.inferred_action_from_account_id,
                        principalTable: "accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_operation_groups_accounts_inferred_action_to_account_id",
                        column: x => x.inferred_action_to_account_id,
                        principalTable: "accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_operation_groups_ledger_transactions_state_version",
                        column: x => x.state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_operation_groups_resources_inferred_action_resource_id",
                        column: x => x.inferred_action_resource_id,
                        principalTable: "resources",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_operation_groups_validators_inferred_action_validator_id",
                        column: x => x.inferred_action_validator_id,
                        principalTable: "validators",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "validator_proposal_records",
                columns: table => new
                {
                    validator_id = table.Column<long>(type: "bigint", nullable: false),
                    epoch = table.Column<long>(type: "bigint", nullable: false),
                    proposals_completed = table.Column<long>(type: "bigint", nullable: false),
                    proposals_missed = table.Column<long>(type: "bigint", nullable: false),
                    last_updated_state_version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validator_proposal_records", x => new { x.epoch, x.validator_id });
                    table.ForeignKey(
                        name: "FK_validator_proposal_record_last_updated_transaction",
                        column: x => x.last_updated_state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version");
                    table.ForeignKey(
                        name: "FK_validator_proposal_records_validators_validator_id",
                        column: x => x.validator_id,
                        principalTable: "validators",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "validator_stake_history",
                columns: table => new
                {
                    from_state_version = table.Column<long>(type: "bigint", nullable: false),
                    validator_id = table.Column<long>(type: "bigint", nullable: false),
                    total_xrd_staked = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    total_stake_units = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    total_prepared_xrd_stake = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    total_prepared_unstake_units = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    total_exiting_xrd_stake = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false),
                    to_state_version = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validator_stake_history", x => new { x.validator_id, x.from_state_version });
                    table.ForeignKey(
                        name: "FK_validator_stake_history_from_transaction",
                        column: x => x.from_state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validator_stake_history_to_transaction",
                        column: x => x.to_state_version,
                        principalTable: "ledger_transactions",
                        principalColumn: "state_version",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_validator_stake_history_validators_validator_id",
                        column: x => x.validator_id,
                        principalTable: "validators",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_resource_balance_substates",
                columns: table => new
                {
                    up_state_version = table.Column<long>(type: "bigint", nullable: false),
                    up_operation_group_index = table.Column<int>(type: "integer", nullable: false),
                    up_operation_index_in_group = table.Column<int>(type: "integer", nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    resource_id = table.Column<long>(type: "bigint", nullable: false),
                    down_state_version = table.Column<long>(type: "bigint", nullable: true),
                    down_operation_group_index = table.Column<int>(type: "integer", nullable: true),
                    down_operation_index_in_group = table.Column<int>(type: "integer", nullable: true),
                    substate_identifier = table.Column<byte[]>(type: "bytea", nullable: false),
                    amount = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_resource_balance_substates", x => new { x.up_state_version, x.up_operation_group_index, x.up_operation_index_in_group });
                    table.UniqueConstraint("AK_account_resource_balance_substates_substate_identifier", x => x.substate_identifier);
                    table.ForeignKey(
                        name: "FK_account_resource_balance_substate_down_operation_group",
                        columns: x => new { x.down_state_version, x.down_operation_group_index },
                        principalTable: "operation_groups",
                        principalColumns: new[] { "state_version", "operation_group_index" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_account_resource_balance_substate_up_operation_group",
                        columns: x => new { x.up_state_version, x.up_operation_group_index },
                        principalTable: "operation_groups",
                        principalColumns: new[] { "state_version", "operation_group_index" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_resource_balance_substates_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_resource_balance_substates_resources_resource_id",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_stake_unit_balance_substates",
                columns: table => new
                {
                    up_state_version = table.Column<long>(type: "bigint", nullable: false),
                    up_operation_group_index = table.Column<int>(type: "integer", nullable: false),
                    up_operation_index_in_group = table.Column<int>(type: "integer", nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    validator_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    down_state_version = table.Column<long>(type: "bigint", nullable: true),
                    down_operation_group_index = table.Column<int>(type: "integer", nullable: true),
                    down_operation_index_in_group = table.Column<int>(type: "integer", nullable: true),
                    substate_identifier = table.Column<byte[]>(type: "bytea", nullable: false),
                    amount = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_stake_unit_balance_substates", x => new { x.up_state_version, x.up_operation_group_index, x.up_operation_index_in_group });
                    table.UniqueConstraint("AK_account_stake_unit_balance_substates_substate_identifier", x => x.substate_identifier);
                    table.ForeignKey(
                        name: "FK_account_stake_unit_balance_substate_down_operation_group",
                        columns: x => new { x.down_state_version, x.down_operation_group_index },
                        principalTable: "operation_groups",
                        principalColumns: new[] { "state_version", "operation_group_index" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_account_stake_unit_balance_substate_up_operation_group",
                        columns: x => new { x.up_state_version, x.up_operation_group_index },
                        principalTable: "operation_groups",
                        principalColumns: new[] { "state_version", "operation_group_index" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_stake_unit_balance_substates_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_stake_unit_balance_substates_validators_validator_id",
                        column: x => x.validator_id,
                        principalTable: "validators",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "account_xrd_stake_balance_substates",
                columns: table => new
                {
                    up_state_version = table.Column<long>(type: "bigint", nullable: false),
                    up_operation_group_index = table.Column<int>(type: "integer", nullable: false),
                    up_operation_index_in_group = table.Column<int>(type: "integer", nullable: false),
                    account_id = table.Column<long>(type: "bigint", nullable: false),
                    validator_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    unlock_epoch = table.Column<long>(type: "bigint", nullable: true),
                    down_state_version = table.Column<long>(type: "bigint", nullable: true),
                    down_operation_group_index = table.Column<int>(type: "integer", nullable: true),
                    down_operation_index_in_group = table.Column<int>(type: "integer", nullable: true),
                    substate_identifier = table.Column<byte[]>(type: "bytea", nullable: false),
                    amount = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_account_xrd_stake_balance_substates", x => new { x.up_state_version, x.up_operation_group_index, x.up_operation_index_in_group });
                    table.UniqueConstraint("AK_account_xrd_stake_balance_substates_substate_identifier", x => x.substate_identifier);
                    table.ForeignKey(
                        name: "FK_account_xrd_stake_balance_substate_down_operation_group",
                        columns: x => new { x.down_state_version, x.down_operation_group_index },
                        principalTable: "operation_groups",
                        principalColumns: new[] { "state_version", "operation_group_index" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_account_xrd_stake_balance_substate_up_operation_group",
                        columns: x => new { x.up_state_version, x.up_operation_group_index },
                        principalTable: "operation_groups",
                        principalColumns: new[] { "state_version", "operation_group_index" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_xrd_stake_balance_substates_accounts_account_id",
                        column: x => x.account_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_account_xrd_stake_balance_substates_validators_validator_id",
                        column: x => x.validator_id,
                        principalTable: "validators",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "resource_data_substates",
                columns: table => new
                {
                    up_state_version = table.Column<long>(type: "bigint", nullable: false),
                    up_operation_group_index = table.Column<int>(type: "integer", nullable: false),
                    up_operation_index_in_group = table.Column<int>(type: "integer", nullable: false),
                    resource_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    is_mutable = table.Column<bool>(type: "boolean", nullable: true),
                    granularity = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: true),
                    owner_id = table.Column<long>(type: "bigint", nullable: true),
                    symbol = table.Column<string>(type: "text", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    icon_url = table.Column<string>(type: "text", nullable: true),
                    down_state_version = table.Column<long>(type: "bigint", nullable: true),
                    down_operation_group_index = table.Column<int>(type: "integer", nullable: true),
                    down_operation_index_in_group = table.Column<int>(type: "integer", nullable: true),
                    substate_identifier = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_resource_data_substates", x => new { x.up_state_version, x.up_operation_group_index, x.up_operation_index_in_group });
                    table.UniqueConstraint("AK_resource_data_substates_substate_identifier", x => x.substate_identifier);
                    table.ForeignKey(
                        name: "FK_resource_data_substate_down_operation_group",
                        columns: x => new { x.down_state_version, x.down_operation_group_index },
                        principalTable: "operation_groups",
                        principalColumns: new[] { "state_version", "operation_group_index" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_resource_data_substate_up_operation_group",
                        columns: x => new { x.up_state_version, x.up_operation_group_index },
                        principalTable: "operation_groups",
                        principalColumns: new[] { "state_version", "operation_group_index" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_resource_data_substates_accounts_owner_id",
                        column: x => x.owner_id,
                        principalTable: "accounts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_resource_data_substates_resources_resource_id",
                        column: x => x.resource_id,
                        principalTable: "resources",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "validator_data_substates",
                columns: table => new
                {
                    up_state_version = table.Column<long>(type: "bigint", nullable: false),
                    up_operation_group_index = table.Column<int>(type: "integer", nullable: false),
                    up_operation_index_in_group = table.Column<int>(type: "integer", nullable: false),
                    validator_id = table.Column<long>(type: "bigint", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    effective_epoch = table.Column<long>(type: "bigint", nullable: true),
                    owner_id = table.Column<long>(type: "bigint", nullable: true),
                    is_registered = table.Column<bool>(type: "boolean", nullable: true),
                    fee_percentage = table.Column<decimal>(type: "numeric", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    allow_delegation = table.Column<bool>(type: "boolean", nullable: true),
                    prepared_is_registered = table.Column<bool>(type: "boolean", nullable: true),
                    prepared_fee_percentage = table.Column<decimal>(type: "numeric", nullable: true),
                    prepared_owner_id = table.Column<long>(type: "bigint", nullable: true),
                    down_state_version = table.Column<long>(type: "bigint", nullable: true),
                    down_operation_group_index = table.Column<int>(type: "integer", nullable: true),
                    down_operation_index_in_group = table.Column<int>(type: "integer", nullable: true),
                    substate_identifier = table.Column<byte[]>(type: "bytea", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validator_data_substates", x => new { x.up_state_version, x.up_operation_group_index, x.up_operation_index_in_group });
                    table.UniqueConstraint("AK_validator_data_substates_substate_identifier", x => x.substate_identifier);
                    table.ForeignKey(
                        name: "FK_validator_data_substate_down_operation_group",
                        columns: x => new { x.down_state_version, x.down_operation_group_index },
                        principalTable: "operation_groups",
                        principalColumns: new[] { "state_version", "operation_group_index" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_validator_data_substate_up_operation_group",
                        columns: x => new { x.up_state_version, x.up_operation_group_index },
                        principalTable: "operation_groups",
                        principalColumns: new[] { "state_version", "operation_group_index" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validator_data_substates_accounts_owner_id",
                        column: x => x.owner_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validator_data_substates_accounts_prepared_owner_id",
                        column: x => x.prepared_owner_id,
                        principalTable: "accounts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validator_data_substates_validators_validator_id",
                        column: x => x.validator_id,
                        principalTable: "validators",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "validator_stake_balance_substates",
                columns: table => new
                {
                    up_state_version = table.Column<long>(type: "bigint", nullable: false),
                    up_operation_group_index = table.Column<int>(type: "integer", nullable: false),
                    up_operation_index_in_group = table.Column<int>(type: "integer", nullable: false),
                    validator_id = table.Column<long>(type: "bigint", nullable: false),
                    epoch = table.Column<long>(type: "bigint", nullable: false),
                    down_state_version = table.Column<long>(type: "bigint", nullable: true),
                    down_operation_group_index = table.Column<int>(type: "integer", nullable: true),
                    down_operation_index_in_group = table.Column<int>(type: "integer", nullable: true),
                    substate_identifier = table.Column<byte[]>(type: "bytea", nullable: false),
                    amount = table.Column<BigInteger>(type: "numeric(1000,0)", precision: 1000, scale: 0, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validator_stake_balance_substates", x => new { x.up_state_version, x.up_operation_group_index, x.up_operation_index_in_group });
                    table.UniqueConstraint("AK_validator_stake_balance_substates_substate_identifier", x => x.substate_identifier);
                    table.ForeignKey(
                        name: "FK_validator_stake_balance_substate_down_operation_group",
                        columns: x => new { x.down_state_version, x.down_operation_group_index },
                        principalTable: "operation_groups",
                        principalColumns: new[] { "state_version", "operation_group_index" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_validator_stake_balance_substate_up_operation_group",
                        columns: x => new { x.up_state_version, x.up_operation_group_index },
                        principalTable: "operation_groups",
                        principalColumns: new[] { "state_version", "operation_group_index" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_validator_stake_balance_substates_validators_validator_id",
                        column: x => x.validator_id,
                        principalTable: "validators",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_account_resource_balance_history_account_id_from_state_vers~",
                table: "account_resource_balance_history",
                columns: new[] { "account_id", "from_state_version" });

            migrationBuilder.CreateIndex(
                name: "IX_account_resource_balance_history_current_balance",
                table: "account_resource_balance_history",
                columns: new[] { "account_id", "resource_id" },
                unique: true,
                filter: "to_state_version is null");

            migrationBuilder.CreateIndex(
                name: "IX_account_resource_balance_history_from_state_version",
                table: "account_resource_balance_history",
                column: "from_state_version");

            migrationBuilder.CreateIndex(
                name: "IX_account_resource_balance_history_resource_id_account_id_fro~",
                table: "account_resource_balance_history",
                columns: new[] { "resource_id", "account_id", "from_state_version" });

            migrationBuilder.CreateIndex(
                name: "IX_account_resource_balance_history_resource_id_from_state_ver~",
                table: "account_resource_balance_history",
                columns: new[] { "resource_id", "from_state_version" });

            migrationBuilder.CreateIndex(
                name: "IX_account_resource_balance_history_to_state_version",
                table: "account_resource_balance_history",
                column: "to_state_version");

            migrationBuilder.CreateIndex(
                name: "IX_account_resource_balance_substate_current_unspent_utxos",
                table: "account_resource_balance_substates",
                columns: new[] { "account_id", "resource_id", "amount" },
                filter: "down_state_version is null")
                .Annotation("Npgsql:IndexInclude", new[] { "substate_identifier" });

            migrationBuilder.CreateIndex(
                name: "IX_account_resource_balance_substates_account_id_resource_id",
                table: "account_resource_balance_substates",
                columns: new[] { "account_id", "resource_id" });

            migrationBuilder.CreateIndex(
                name: "IX_account_resource_balance_substates_down_state_version_down_~",
                table: "account_resource_balance_substates",
                columns: new[] { "down_state_version", "down_operation_group_index" });

            migrationBuilder.CreateIndex(
                name: "IX_account_resource_balance_substates_resource_id_account_id",
                table: "account_resource_balance_substates",
                columns: new[] { "resource_id", "account_id" });

            migrationBuilder.CreateIndex(
                name: "IX_account_stake_unit_balance_substates_account_id_validator_id",
                table: "account_stake_unit_balance_substates",
                columns: new[] { "account_id", "validator_id" });

            migrationBuilder.CreateIndex(
                name: "IX_account_stake_unit_balance_substates_down_state_version_dow~",
                table: "account_stake_unit_balance_substates",
                columns: new[] { "down_state_version", "down_operation_group_index" });

            migrationBuilder.CreateIndex(
                name: "IX_account_stake_unit_balance_substates_validator_id_account_id",
                table: "account_stake_unit_balance_substates",
                columns: new[] { "validator_id", "account_id" });

            migrationBuilder.CreateIndex(
                name: "IX_account_transactions_state_version",
                table: "account_transactions",
                column: "state_version");

            migrationBuilder.CreateIndex(
                name: "IX_account_validator_stake_history_account_id_from_state_versi~",
                table: "account_validator_stake_history",
                columns: new[] { "account_id", "from_state_version" });

            migrationBuilder.CreateIndex(
                name: "IX_account_validator_stake_history_current_stake",
                table: "account_validator_stake_history",
                columns: new[] { "account_id", "validator_id" },
                unique: true,
                filter: "to_state_version is null");

            migrationBuilder.CreateIndex(
                name: "IX_account_validator_stake_history_from_state_version",
                table: "account_validator_stake_history",
                column: "from_state_version");

            migrationBuilder.CreateIndex(
                name: "IX_account_validator_stake_history_to_state_version",
                table: "account_validator_stake_history",
                column: "to_state_version");

            migrationBuilder.CreateIndex(
                name: "IX_account_validator_stake_history_validator_id_account_id_fro~",
                table: "account_validator_stake_history",
                columns: new[] { "validator_id", "account_id", "from_state_version" });

            migrationBuilder.CreateIndex(
                name: "IX_account_validator_stake_history_validator_id_from_state_ver~",
                table: "account_validator_stake_history",
                columns: new[] { "validator_id", "from_state_version" });

            migrationBuilder.CreateIndex(
                name: "IX_account_xrd_stake_balance_substates_account_id_validator_id",
                table: "account_xrd_stake_balance_substates",
                columns: new[] { "account_id", "validator_id" });

            migrationBuilder.CreateIndex(
                name: "IX_account_xrd_stake_balance_substates_down_state_version_down~",
                table: "account_xrd_stake_balance_substates",
                columns: new[] { "down_state_version", "down_operation_group_index" });

            migrationBuilder.CreateIndex(
                name: "IX_account_xrd_stake_balance_substates_validator_id_account_id",
                table: "account_xrd_stake_balance_substates",
                columns: new[] { "validator_id", "account_id" });

            migrationBuilder.CreateIndex(
                name: "IX_accounts_address",
                table: "accounts",
                column: "address",
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "id" });

            migrationBuilder.CreateIndex(
                name: "IX_accounts_from_state_version",
                table: "accounts",
                column: "from_state_version");

            migrationBuilder.CreateIndex(
                name: "IX_ledger_transactions_epoch_round_in_epoch",
                table: "ledger_transactions",
                columns: new[] { "epoch", "round_in_epoch" },
                unique: true,
                filter: "is_start_of_round = true");

            migrationBuilder.CreateIndex(
                name: "IX_ledger_transactions_timestamp",
                table: "ledger_transactions",
                column: "timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_operation_groups_inferred_action_from_account_id",
                table: "operation_groups",
                column: "inferred_action_from_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_operation_groups_inferred_action_resource_id",
                table: "operation_groups",
                column: "inferred_action_resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_operation_groups_inferred_action_to_account_id",
                table: "operation_groups",
                column: "inferred_action_to_account_id");

            migrationBuilder.CreateIndex(
                name: "IX_operation_groups_inferred_action_validator_id",
                table: "operation_groups",
                column: "inferred_action_validator_id");

            migrationBuilder.CreateIndex(
                name: "IX_resource_data_substates_down_state_version_down_operation_g~",
                table: "resource_data_substates",
                columns: new[] { "down_state_version", "down_operation_group_index" });

            migrationBuilder.CreateIndex(
                name: "IX_resource_data_substates_owner_id",
                table: "resource_data_substates",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_resource_data_substates_resource_id",
                table: "resource_data_substates",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_resource_supply_history_current_supply",
                table: "resource_supply_history",
                column: "resource_id",
                unique: true,
                filter: "to_state_version is null");

            migrationBuilder.CreateIndex(
                name: "IX_resource_supply_history_from_state_version",
                table: "resource_supply_history",
                column: "from_state_version");

            migrationBuilder.CreateIndex(
                name: "IX_resource_supply_history_to_state_version",
                table: "resource_supply_history",
                column: "to_state_version");

            migrationBuilder.CreateIndex(
                name: "IX_resources_from_state_version",
                table: "resources",
                column: "from_state_version");

            migrationBuilder.CreateIndex(
                name: "IX_resources_rri",
                table: "resources",
                column: "rri",
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "id" });

            migrationBuilder.CreateIndex(
                name: "IX_validator_data_substates_down_state_version_down_operation_~",
                table: "validator_data_substates",
                columns: new[] { "down_state_version", "down_operation_group_index" });

            migrationBuilder.CreateIndex(
                name: "IX_validator_data_substates_owner_id",
                table: "validator_data_substates",
                column: "owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_validator_data_substates_prepared_owner_id",
                table: "validator_data_substates",
                column: "prepared_owner_id");

            migrationBuilder.CreateIndex(
                name: "IX_validator_data_substates_validator_id",
                table: "validator_data_substates",
                column: "validator_id");

            migrationBuilder.CreateIndex(
                name: "IX_validator_proposal_records_last_updated_state_version",
                table: "validator_proposal_records",
                column: "last_updated_state_version");

            migrationBuilder.CreateIndex(
                name: "IX_validator_proposal_records_validator_id_epoch",
                table: "validator_proposal_records",
                columns: new[] { "validator_id", "epoch" });

            migrationBuilder.CreateIndex(
                name: "IX_validator_stake_balance_substates_down_state_version_down_o~",
                table: "validator_stake_balance_substates",
                columns: new[] { "down_state_version", "down_operation_group_index" });

            migrationBuilder.CreateIndex(
                name: "IX_validator_stake_balance_substates_epoch_validator_id",
                table: "validator_stake_balance_substates",
                columns: new[] { "epoch", "validator_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_validator_stake_balance_substates_validator_id",
                table: "validator_stake_balance_substates",
                column: "validator_id");

            migrationBuilder.CreateIndex(
                name: "IX_validator_stake_history_current_stake",
                table: "validator_stake_history",
                column: "validator_id",
                unique: true,
                filter: "to_state_version is null");

            migrationBuilder.CreateIndex(
                name: "IX_validator_stake_history_from_state_version",
                table: "validator_stake_history",
                column: "from_state_version");

            migrationBuilder.CreateIndex(
                name: "IX_validator_stake_history_to_state_version",
                table: "validator_stake_history",
                column: "to_state_version");

            migrationBuilder.CreateIndex(
                name: "IX_validators_address",
                table: "validators",
                column: "address",
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "id" });

            migrationBuilder.CreateIndex(
                name: "IX_validators_from_state_version",
                table: "validators",
                column: "from_state_version");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "account_resource_balance_history");

            migrationBuilder.DropTable(
                name: "account_resource_balance_substates");

            migrationBuilder.DropTable(
                name: "account_stake_unit_balance_substates");

            migrationBuilder.DropTable(
                name: "account_transactions");

            migrationBuilder.DropTable(
                name: "account_validator_stake_history");

            migrationBuilder.DropTable(
                name: "account_xrd_stake_balance_substates");

            migrationBuilder.DropTable(
                name: "mempool_transactions");

            migrationBuilder.DropTable(
                name: "network_configuration");

            migrationBuilder.DropTable(
                name: "nodes");

            migrationBuilder.DropTable(
                name: "resource_data_substates");

            migrationBuilder.DropTable(
                name: "resource_supply_history");

            migrationBuilder.DropTable(
                name: "validator_data_substates");

            migrationBuilder.DropTable(
                name: "validator_proposal_records");

            migrationBuilder.DropTable(
                name: "validator_stake_balance_substates");

            migrationBuilder.DropTable(
                name: "validator_stake_history");

            migrationBuilder.DropTable(
                name: "operation_groups");

            migrationBuilder.DropTable(
                name: "accounts");

            migrationBuilder.DropTable(
                name: "resources");

            migrationBuilder.DropTable(
                name: "validators");

            migrationBuilder.DropTable(
                name: "ledger_transactions");

            migrationBuilder.DropTable(
                name: "raw_transactions");
        }
    }
}
