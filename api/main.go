package main

import (
	"database/sql"
	"fmt"
	"log"
	"time"

	//"encoding/json"

	"github.com/gin-contrib/cors"
	"github.com/gin-gonic/gin"
	_ "github.com/go-sql-driver/mysql"
)

var DB *sql.DB

type CreateGameRequestBody struct {
	TimeLimit     int
	TimeIncrement int
}

type GetGameRequestBody struct {
	GameID int
}

/* type GameState struct {
	GameState            string
	PlayerColor          int
	PlayerRemainingTime  int
	PreviousMoveNumber   int
	PreviousTargetSquare int
} */
/* 	TimeLimit          int
TimeIncrement      int `json:"time_increment,omitempty"`
GameStatus         int `json:"game_status,omitempty"`
WhiteTimeRemaining int `json:"white_time_remaining,omitempty"`
BlackTimeRemaining int `json:"black_time_remaining,omitempty"` */

type NullableMove struct {
	PlayerColor         sql.NullInt64  `json:"playerColor,omitempty"`
	MoveNumber          sql.NullInt64  `json:"moveNumber,omitempty"`
	TargetSquare        sql.NullInt64  `json:"targetSquare,omitempty"`
	PlayerRemainingTime sql.NullInt64  `json:"playerRemaining_time,omitempty"`
	ResultingState      sql.NullString `json:"resultingState,omitempty"`
}

type Move struct {
	PlayerColor         int    `json:"playerColor,omitempty"`
	MoveNumber          int    `json:"moveNumber,omitempty"`
	TargetSquare        int    `json:"targetSquare,omitempty"`
	PlayerRemainingTime int    `json:"playerRemaining_time,omitempty"`
	ResultingState      string `json:"resultingState,omitempty"`
}

type Game struct {
	ID                 int64  `json:"id,primaryKey:type:uuid"`
	TimeLimit          int    `json:"timeLimit,omitempty"`
	TimeIncrement      int    `json:"timeIncrement,omitempty"`
	GameStatus         int    `json:"gameStatus,omitempty"`
	WhiteTimeRemaining int    `json:"whiteTimeRemaining,omitempty"`
	BlackTimeRemaining int    `json:"blackTimeRemaining,omitempty"`
	Moves              []Move `json:"moves,omitempty"`
}

/* type Move struct {
	TimeLimit          int `json:"time_limit,omitempty"`
	TimeIncrement      int `json:"time_increment,omitempty"`
	GameStatus         int `json:"game_status,omitempty"`
	WhiteTimeRemaining int `json:"white_time_remaining,omitempty"`
	BlackTimeRemaining int `json:"black_time_remaining,omitempty"`
} */

func main() {
	db, err := sql.Open("mysql",
		"root:hej.@tcp(127.0.0.1:3306)/othello")
	if err != nil {
		log.Fatal(err)
	}
	err = db.Ping()
	if err != nil {
		log.Fatal(err)
	}
	DB = db
	defer db.Close()

	router := gin.Default()

	router.POST("/game/createGame", createGame)
	// router.POST("/game", makeMove)
	// router.POST("/game", revertMove)
	// router.POST("/game", surrender)
	// router.POST("/game", pass)
	router.GET("/game/getGame", getGame)
	// router.GET("/game", getTime)

	router.Use(cors.New(cors.Config{
		AllowOrigins:     []string{"*"},
		AllowMethods:     []string{"*"},
		AllowHeaders:     []string{"Origin"},
		ExposeHeaders:    []string{"Content-Length"},
		AllowCredentials: true,
		AllowOriginFunc: func(origin string) bool {
			return origin == "https://github.com"
		},
		MaxAge: 12 * time.Hour,
	}))

	router.Run(":1016")
}

func createGame(c *gin.Context) {
	var requestBody CreateGameRequestBody

	if err := c.BindJSON(&requestBody); err != nil {
		panic(err)
	}

	ins, err := DB.Prepare("INSERT INTO games (time_limit, time_increment, game_status, white_time_remaining, black_time_remaining) VALUES (?,?,?,?,?)")
	if err != nil {
		panic(err)
	}

	defer ins.Close()

	res, err := ins.Exec(
		requestBody.TimeLimit,
		requestBody.TimeIncrement,
		5,
		requestBody.TimeLimit,
		requestBody.TimeLimit,
	)

	id, err := res.LastInsertId()

	rowsAffec, _ := res.RowsAffected()
	if err != nil || rowsAffec != 1 {
		panic(err)
	}

	/* 	game := Game{
		ID:                 id,
		TimeLimit:          requestBody.TimeLimit,
		TimeIncrement:      requestBody.TimeIncrement,
		GameStatus:         5,
		WhiteTimeRemaining: requestBody.TimeLimit,
		BlackTimeRemaining: requestBody.TimeLimit,
		Moves:              []Move{},
	} */
	c.JSON(200, id)
}

func getGame(c *gin.Context) {
	query := `
    SELECT g.id, g.time_limit, g.time_increment, g.game_status, g.white_time_remaining, g.black_time_remaining, 
	moves.resulting_state, moves.target_square, moves.move_number,
	moves.player_color,
	moves.player_remaining_time
    FROM games AS g
    LEFT JOIN moves ON g.id = moves.game_id
    WHERE g.id= ?
  `

	gameID := c.Query("GameID")

	rows, err := DB.Query(query, gameID)
	if err != nil {
		fmt.Println("ERROR 2")
		panic(err)
	}

	game := &Game{}
	for rows.Next() {
		nullableMove := &NullableMove{}
		err = rows.Scan(
			&game.ID,
			&game.TimeLimit, &game.TimeIncrement, &game.GameStatus, &game.WhiteTimeRemaining, &game.BlackTimeRemaining,
			&nullableMove.ResultingState,
			&nullableMove.TargetSquare,
			&nullableMove.MoveNumber,
			&nullableMove.PlayerColor,
			&nullableMove.PlayerRemainingTime,
		)
		if err != nil {
			fmt.Println(err)
			panic(err)
		}
		if nullableMove.ResultingState.Valid && nullableMove.TargetSquare.Valid && nullableMove.MoveNumber.Valid && nullableMove.PlayerColor.Valid && nullableMove.PlayerRemainingTime.Valid {
			game.Moves = append(game.Moves, Move{
				PlayerColor:         int(nullableMove.PlayerColor.Int64),
				MoveNumber:          int(nullableMove.MoveNumber.Int64),
				TargetSquare:        int(nullableMove.TargetSquare.Int64),
				PlayerRemainingTime: int(nullableMove.PlayerRemainingTime.Int64),
				ResultingState:      string(nullableMove.ResultingState.String),
			})
		}
	}

	c.JSON(200, game)
}
