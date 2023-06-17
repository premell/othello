
// still kinda confused how this works compared to enums


export const DIRECTION_VALUES = {
  UP: -8,
  UP_RIGHT: -7,
  RIGHT: 1,
  DOWN_RIGHT: 9,
  DOWN: 8,
  DOWN_LEFT: 7,
  LEFT: -1,
  LEFT_UP: -9,
} as const;
export type Direction = (typeof DIRECTION_VALUES)[keyof typeof DIRECTION_VALUES];

export const PLAYER_ACTION_VALUES = {
  DRAW: 0,
  TAKEBACK: 1,
  REMATCH: 2
} as const;
export type PlayerAction = (typeof PLAYER_ACTION_VALUES)[keyof typeof PLAYER_ACTION_VALUES];

export const ACTION_TYPE_VALUES = {
  REQUEST: 0,
  CANCEL: 1,
  REJECT: 2,
  ACCEPT: 3,
} as const;
export type ActionType = (typeof ACTION_TYPE_VALUES)[keyof typeof ACTION_TYPE_VALUES];

export const GAME_STATUS_VALUES = {
  PLAYING: 0,
  SURRENDERED: 1,
  WON_TIME: 2,
  WON_BY_MARKS: 3,
  CANCELLED: 4,
  DRAW: 5,
} as const;
export type GameStatus = (typeof GAME_STATUS_VALUES)[keyof typeof GAME_STATUS_VALUES];

export const COLOR_VALUES = {
  NONE: -1,
  BLACK: 0,
  WHITE: 1,
} as const;
export type Color = (typeof COLOR_VALUES)[keyof typeof COLOR_VALUES];

// mostly to throw error if its not a color atm, really shouldnt be needed
export const numberToColor = (value: number):Color => {
  const colorEntries = Object.entries(COLOR_VALUES);
  const foundColor = colorEntries.find(([_, colorValue]) => colorValue === value);

  if (foundColor) {
    return value as Color;
  }

  throw new Error('Invalid color number provided');
};

export const PLAYER_VALUES = {
  USER: 0,
  OPPONENT: 1,
} as const;
export type Player = (typeof PLAYER_VALUES)[keyof typeof PLAYER_VALUES];
